namespace PrismMod;

public sealed class CommonRummage : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/hiddencard.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public CommonRummage() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            await PrismRandomCardHelper.AddRandomCardToHand(ctx, base.Owner, card => card.Rarity == CardRarity.Common);
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Cards.UpgradeValueBy(1m);
}
