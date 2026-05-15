namespace PrismMod;

public sealed class FieldProcurementPower : PrismPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }

        Flash();
        await PrismRandomCardHelper.AddPlayableOtherCharacterCardToHand(choiceContext, player);
        await PowerCmd.Decrement(this);
    }
}
