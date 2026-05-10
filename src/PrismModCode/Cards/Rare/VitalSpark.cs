using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class VitalSpark : PrismCard
{
    public VitalSpark() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/vitalspark.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VitalSparkPower>(1m),
        new EnergyVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [base.EnergyHoverTip];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<VitalSparkPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars["VitalSparkPower"].BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
