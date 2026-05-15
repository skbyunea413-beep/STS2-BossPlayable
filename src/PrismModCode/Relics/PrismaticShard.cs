using MegaCrit.Sts2.Core.Entities.Relics;

namespace PrismMod;

public sealed class PrismaticShard : PrismRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    internal void ApplyGeneratedCardModifiers(CardModel card)
    {
        if (card.Owner != base.Owner) return;

        card.AddKeyword(CardKeyword.Exhaust);

        if (card.EnergyCost.CostsX) return;
        if (!PrismRandomCardHelper.IsOtherCharacterCard(card)) return;

        Flash();
        card.EnergyCost.AddThisTurn(-1, reduceOnly: true);
    }
}
