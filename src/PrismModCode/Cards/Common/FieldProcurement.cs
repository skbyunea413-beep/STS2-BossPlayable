namespace PrismMod;

public sealed class FieldProcurement : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/hiddencard.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public FieldProcurement() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            await PrismRandomCardHelper.AddRandomCardToHand(ctx, base.Owner, IsEligibleFieldProcurementCard);
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Cards.UpgradeValueBy(1m);

    private bool IsEligibleFieldProcurementCard(CardModel card)
    {
        return PrismRandomCardHelper.IsOtherCharacterCard(card)
            && PrismRandomCardHelper.IsPlayableThisTurnAfterShard(base.Owner, card);
    }
}
