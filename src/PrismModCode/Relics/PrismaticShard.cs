using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Runs;

namespace PrismMod;

public sealed class PrismaticShard : PrismRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override CardCreationOptions ModifyCardRewardCreationOptions(Player player, CardCreationOptions options)
    {
        if (player != base.Owner) return options;
        if (options.Flags.HasFlag(CardCreationFlags.NoCardPoolModifications)) return options;
        if (!options.Flags.HasFlag(CardCreationFlags.IsCardReward)) return options;
        if (options.CustomCardPool != null) return options;
        if (options.CardPools.All(pool => pool.IsColorless)) return options;

        var pools = options.CardPools
            .Concat([ModelDb.CardPool<PrismCardPool>()])
            .Distinct();
        return options.WithCardPools(pools, options.CardPoolFilter);
    }

    internal void ApplyGeneratedCardModifiers(CardModel card)
    {
        if (card.Owner != base.Owner) return;

        card.AddKeyword(CardKeyword.Exhaust);

        if (card.EnergyCost.CostsX) return;
        if (!IsOtherCharacterCard(card)) return;

        Flash();
        card.EnergyCost.AddThisTurn(-1, reduceOnly: true);
    }

    private static bool IsOtherCharacterCard(CardModel card)
    {
        return ModelDb.AllCharacterCardPools.Contains(card.Pool);
    }
}
