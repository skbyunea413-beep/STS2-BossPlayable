namespace PrismMod;

public sealed class GravityCapacitorPower : PrismPower
{
    internal decimal Block { get; set; } = 5m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner.Player;
        if (player == null ||
            cardPlay.Card.Owner != player ||
            cardPlay.IsAutoPlay ||
            !PrismRandomCardHelper.HasCostAtLeast(cardPlay.Card, 2))
        {
            return;
        }

        Flash();
        await CreatureCmd.GainBlock(base.Owner, Block * base.Amount, ValueProp.Move, null);
    }
}
