namespace PrismMod;

public sealed class Fold : PrismCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
    ];

    public Fold() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        for (int playedCards = 0; playedCards < base.DynamicVars.Cards.IntValue; playedCards++)
        {
            var card = PileType.Hand.GetPile(base.Owner).Cards
                .FirstOrDefault(CanAutoPlayNow);
            if (card == null)
            {
                break;
            }

            await AutoPlayWithValidTarget(ctx, card);
        }

        PlayerCmd.EndTurn(base.Owner, canBackOut: false);
    }

    protected override void OnUpgrade() => base.DynamicVars.Cards.UpgradeValueBy(1m);
}
