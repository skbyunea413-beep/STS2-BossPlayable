using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class WarningColor : PrismCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("WeakPower", 1m),
        new DamageVar(0m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
    ];

    public WarningColor() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<WarningColorPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars["WeakPower"].BaseValue,
            base.Owner.Creature,
            this);
        if (power != null)
        {
            power.Damage = base.DynamicVars.Damage.BaseValue;
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(3m);
}
