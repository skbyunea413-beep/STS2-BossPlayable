namespace PrismMod;

public sealed class PulsatePower : PrismPower
{
    private const int Interval = 4;
    private int _turns;

    internal decimal Block { get; set; } = 12m;
    internal decimal Strength { get; set; } = 4m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12m, ValueProp.Move),
        new PowerVar<StrengthPower>(4m),
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (base.Owner.IsDead || player != base.Owner.Player)
        {
            return;
        }

        _turns++;
        if (_turns % Interval != 0)
        {
            return;
        }

        Flash();
        await CreatureCmd.GainBlock(base.Owner, Block * base.Amount, ValueProp.Move, null);
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            base.Owner,
            Strength * base.Amount,
            base.Owner,
            null);
    }
}
