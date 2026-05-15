namespace PrismMod;

public sealed class AfterglowCompression : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/afterglowcompression_edit.png";

    protected override bool ShouldGlowGoldInternal => PrismRandomCardHelper.HasHandCardWithCostAtLeast(base.Owner, 2);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    public AfterglowCompression() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, base.DynamicVars.Cards.BaseValue, base.Owner);
        foreach (var card in PileType.Hand.GetPile(base.Owner).Cards)
        {
            if (!card.EnergyCost.CostsX &&
                card.EnergyCost.GetWithModifiers(CostModifiers.All) >= 2)
            {
                card.EnergyCost.SetThisTurn(1, reduceOnly: true);
            }
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Cards.UpgradeValueBy(1m);
}
