using MegaCrit.Sts2.Core.Combat;

namespace PrismMod;

public sealed class MirrorScreenPower : PrismPower
{
    private bool _triggered;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (_triggered ||
            target != base.Owner ||
            dealer == null ||
            result.UnblockedDamage <= 0 ||
            !props.IsPoweredAttack())
        {
            return;
        }

        _triggered = true;
        Flash();
        await CreatureCmd.Damage(
            choiceContext,
            dealer,
            result.UnblockedDamage,
            ValueProp.Unpowered,
            base.Owner,
            null);
        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
