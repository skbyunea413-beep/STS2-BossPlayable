namespace PrismMod;

public sealed class BorrowedMoment : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/hiddencard.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public BorrowedMoment() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            await PrismRandomCardHelper.AddRandomCardToHand(
                ctx,
                base.Owner,
                card => PrismRandomCardHelper.IsOtherCharacterCard(card)
                    && PrismRandomCardHelper.IsPlayableThisTurnAfterShard(base.Owner, card));
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Cards.UpgradeValueBy(1m);
}
