namespace PrismMod;

public sealed class LensRunawayPower : PrismPower
{
    internal decimal DamageIncrease { get; set; } = 3m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move)];

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
        await AttackIntentPower.IncreaseAll(choiceContext, player, DamageIncrease * base.Amount, null);
    }
}
