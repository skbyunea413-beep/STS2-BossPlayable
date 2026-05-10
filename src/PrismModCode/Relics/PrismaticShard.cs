using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;
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

    public override bool TryModifyCardRewardOptionsLate(Player player, List<CardCreationResult> cardRewards, CardCreationOptions options)
    {
        if (player != base.Owner) return false;
        if (!options.Flags.HasFlag(CardCreationFlags.IsCardReward)) return false;

        var changed = false;
        var rng = player.PlayerRng.Rewards;
        foreach (var cardReward in cardRewards)
        {
            var card = cardReward.Card;
            var enchantments = RewardEnchantments()
                .Where(enchantment => enchantment.CanEnchant(card))
                .ToList();
            if (enchantments.Count == 0) continue;

            var selectedEnchantment = rng.NextItem(enchantments);
            if (selectedEnchantment == null) continue;

            var enchantedCard = base.Owner.RunState.CloneCard(card);
            var enchantment = selectedEnchantment.ToMutable();
            CardCmd.Enchant(enchantment, enchantedCard, rng.NextInt(1, 4));
            cardReward.ModifyCard(enchantedCard, this);
            changed = true;
        }

        return changed;
    }

    private static IEnumerable<EnchantmentModel> RewardEnchantments()
    {
        yield return ModelDb.Enchantment<Adroit>();
        yield return ModelDb.Enchantment<Corrupted>();
        yield return ModelDb.Enchantment<Glam>();
        yield return ModelDb.Enchantment<Goopy>();
        yield return ModelDb.Enchantment<Imbued>();
        yield return ModelDb.Enchantment<Inky>();
        yield return ModelDb.Enchantment<Instinct>();
        yield return ModelDb.Enchantment<Momentum>();
        yield return ModelDb.Enchantment<Nimble>();
        yield return ModelDb.Enchantment<PerfectFit>();
        yield return ModelDb.Enchantment<RoyallyApproved>();
        yield return ModelDb.Enchantment<Sharp>();
        yield return ModelDb.Enchantment<Slither>();
        yield return ModelDb.Enchantment<SlumberingEssence>();
        yield return ModelDb.Enchantment<SoulsPower>();
        yield return ModelDb.Enchantment<Sown>();
        yield return ModelDb.Enchantment<Spiral>();
        yield return ModelDb.Enchantment<Steady>();
        yield return ModelDb.Enchantment<Swift>();
        yield return ModelDb.Enchantment<TezcatarasEmber>();
        yield return ModelDb.Enchantment<Vigorous>();
    }
}
