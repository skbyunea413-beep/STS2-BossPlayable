namespace PrismMod;

public sealed class RadiantGamblePower : PrismPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner.Player;
        if (player == null ||
            cardPlay.Card.Owner != player ||
            cardPlay.IsAutoPlay ||
            cardPlay.Card.Type != CardType.Attack)
        {
            return;
        }

        var generatedCard = PrismRandomCardHelper.CreateRandomCard(player, card => !card.EnergyCost.CostsX);
        if (generatedCard == null)
        {
            return;
        }

        Flash();
        int cost = generatedCard.EnergyCost.GetWithModifiers(CostModifiers.All);
        await PrismRandomCardHelper.AutoPlayGeneratedCard(choiceContext, generatedCard);
        await PowerCmd.ModifyAmount(choiceContext, this, -cost, base.Owner, null);
    }

}
