namespace PrismMod;

public sealed class DopaminePower : PrismPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner.Player;
        if (player == null ||
            cardPlay.Card.Owner != player ||
            base.Owner.IsDead)
        {
            return;
        }

        Flash();
        var card = await PrismRandomCardHelper.AddRandomCardToHand(choiceContext, player);
        card?.SetToFreeThisTurn();

        await CreatureCmd.Damage(
            choiceContext,
            base.Owner,
            base.Amount,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            cardPlay.Card);
    }
}
