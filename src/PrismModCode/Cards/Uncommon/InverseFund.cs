namespace PrismMod;

public sealed class InverseFund : PrismCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
    ];

    public InverseFund() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<InverseFundPower>(
            ctx,
            base.Owner.Creature,
            1,
            base.Owner.Creature,
            this);

        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            var card = await DrawCostlyCard(ctx);
            if (card == null)
            {
                break;
            }
        }
    }

    private async Task<CardModel?> DrawCostlyCard(PlayerChoiceContext ctx)
    {
        var drawPile = PileType.Draw.GetPile(base.Owner);
        var card = FindCostlyCard(drawPile);
        if (card == null)
        {
            await CardPileCmd.ShuffleIfNecessary(ctx, base.Owner);
            card = FindCostlyCard(drawPile);
        }

        if (card == null)
        {
            return null;
        }

        await CardPileCmd.Add(card, PileType.Hand);
        return card;
    }

    private static CardModel? FindCostlyCard(CardPile drawPile) =>
        drawPile.Cards.FirstOrDefault(card => PrismRandomCardHelper.HasCostAtLeast(card, 2));

    protected override void OnUpgrade() => base.EnergyCost.UpgradeBy(-1);
}
