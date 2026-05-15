namespace PrismMod;

public sealed class BorrowedFootworkPower : PrismPower
{
    private bool _triggeredThisTurn;

    internal decimal Block { get; set; } = 5m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    public override Task AfterEnergyReset(Player player)
    {
        if (player == base.Owner.Player)
        {
            _triggeredThisTurn = false;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner.Player;
        if (_triggeredThisTurn ||
            player == null ||
            cardPlay.Card.Owner != player ||
            cardPlay.IsAutoPlay ||
            !PrismRandomCardHelper.IsOtherCharacterCard(cardPlay.Card))
        {
            return;
        }

        _triggeredThisTurn = true;
        Flash();
        await CreatureCmd.GainBlock(base.Owner, Block * base.Amount, ValueProp.Move, null);
    }
}
