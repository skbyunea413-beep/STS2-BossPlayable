namespace PrismMod;

public sealed class PrismaticSpread : PrismCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    public PrismaticSpread() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AddOtherCharacterCardToPile(ctx, PileType.Hand);
        await AddOtherCharacterCardToPile(ctx, PileType.Draw, CardPilePosition.Random);
        await AddOtherCharacterCardToPile(ctx, PileType.Discard, CardPilePosition.Random);
        await CardPileCmd.Draw(ctx, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade() => base.EnergyCost.UpgradeBy(-1);

    private async Task AddOtherCharacterCardToPile(
        PlayerChoiceContext ctx,
        PileType pileType,
        CardPilePosition position = CardPilePosition.Bottom)
    {
        var card = PrismRandomCardHelper.CreateOtherCharacterCard(base.Owner);
        if (card == null)
        {
            return;
        }

        base.Owner.Relics.OfType<PrismaticShard>().FirstOrDefault()?.ApplyGeneratedCardModifiers(card);
        await CardPileCmd.AddGeneratedCardToCombat(card, pileType, base.Owner, position);
    }
}
