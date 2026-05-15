using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class AttackIntentWeakPower : PrismPower
{
    private Creature? _target;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
        HoverTipFactory.FromPower<WeakPower>(),
    ];

    public void SetTarget(Creature target)
    {
        _target = target;
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }

        Flash();
        if (_target is { IsAlive: true } && base.Owner.CombatState?.HittableEnemies.Contains(_target) == true)
        {
            await PowerCmd.Apply<WeakPower>(
                new BlockingPlayerChoiceContext(),
                _target,
                base.Amount,
                base.Owner,
                null);
        }

        await PowerCmd.Remove(this);
    }
}
