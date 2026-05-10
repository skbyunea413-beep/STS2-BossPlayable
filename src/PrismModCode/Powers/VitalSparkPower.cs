using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class VitalSparkPower : PrismPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath =>
        ModelDb.Power<global::MegaCrit.Sts2.Core.Models.Powers.VitalSparkPower>().IconPath;

    public override string? CustomBigIconPath =>
        ModelDb.Power<global::MegaCrit.Sts2.Core.Models.Powers.VitalSparkPower>().ResolvedBigIconPath;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    public override async Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != base.Owner || dealer == null || !props.IsPoweredAttack())
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<EnergyNextTurnPower>(
            choiceContext,
            base.Owner,
            base.Amount * base.DynamicVars.Energy.IntValue,
            base.Owner,
            null);
    }
}
