namespace PrismMod;

public sealed class BorrowedOrbitPower : PrismPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner.Player;
        if (player == null ||
            cardPlay.Card.Owner != player ||
            cardPlay.IsAutoPlay ||
            !PrismRandomCardHelper.IsOtherCharacterCard(cardPlay.Card))
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            choiceContext,
            base.Owner,
            base.Amount,
            base.Owner,
            null);
    }
}
