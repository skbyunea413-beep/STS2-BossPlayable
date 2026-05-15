namespace PrismMod;

public sealed class LensRunaway : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/lensrunaway.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    public LensRunaway() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<LensRunawayPower>(
            ctx,
            base.Owner.Creature,
            1,
            base.Owner.Creature,
            this);
        if (power != null)
        {
            power.DamageIncrease = base.DynamicVars.Damage.BaseValue;
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(3m);
}
