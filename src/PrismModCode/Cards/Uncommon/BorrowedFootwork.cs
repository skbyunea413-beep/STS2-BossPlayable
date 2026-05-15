namespace PrismMod;

public sealed class BorrowedFootwork : PrismCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    public BorrowedFootwork() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<BorrowedFootworkPower>(
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
