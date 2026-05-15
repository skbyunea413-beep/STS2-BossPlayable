namespace PrismMod;

public sealed class KaleidoscopeHeartPower : PrismPower
{
    private int _triggersThisTurn;

    internal decimal Block { get; set; } = 5m;
    internal int Cards { get; set; } = 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new CardsVar(1),
    ];

    public override Task AfterEnergyReset(Player player)
    {
        if (player == base.Owner.Player)
        {
            _triggersThisTurn = 0;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        var player = base.Owner.Player;
        if (_triggersThisTurn >= base.Amount ||
            player == null ||
            creator != player ||
            card.Owner != player ||
            card.Pile?.Type != PileType.Hand)
        {
            return;
        }

        _triggersThisTurn++;
        Flash();
        await CreatureCmd.GainBlock(base.Owner, Block, ValueProp.Move, null);
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), Cards, player);
    }
}
