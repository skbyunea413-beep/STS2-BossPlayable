namespace PrismMod;

public sealed class PerfectRefraction : PrismCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
    ];

    internal override bool IsBlockedByMissingRequiredPile =>
        !PrismRandomCardHelper.HasReclaimableCardInExhaust(base.Owner);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public PerfectRefraction() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = PrismRandomCardHelper.ReclaimableCardsInExhaust(base.Owner).ToList();
        if (cards.Count == 0)
        {
            return;
        }

        List<CardModel> selected = [];
        for (int i = 0; i < base.DynamicVars.Cards.IntValue && cards.Count > 0; i++)
        {
            var card = base.Owner.RunState.Rng.CombatCardSelection.NextItem(cards);
            if (card == null)
            {
                break;
            }

            cards.Remove(card);
            selected.Add(card);
        }

        await CardPileCmd.Add(selected, PileType.Hand);
    }

    protected override void OnUpgrade() => base.DynamicVars.Cards.UpgradeValueBy(1m);
}
