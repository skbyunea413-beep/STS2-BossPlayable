namespace PrismMod;

public sealed class GravityCapacitor : PrismCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    public GravityCapacitor() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<GravityCapacitorPower>(
            ctx,
            base.Owner.Creature,
            1,
            base.Owner.Creature,
            this);
        if (power != null)
        {
            power.Block = base.DynamicVars.Block.BaseValue;
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(2m);
}
