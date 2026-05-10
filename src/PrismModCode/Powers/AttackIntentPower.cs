using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class AttackIntentPower : PrismPower
{
    private const int Repeat = 3;
    private Creature? _target;
    private bool _targetAllEnemies;

    public override PowerType Type => PowerType.Buff;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new RepeatVar(Repeat),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
    ];

    public void SetTarget(Creature target)
    {
        _target = target;
        _targetAllEnemies = false;
    }

    public void SetTargetAllEnemies()
    {
        _target = null;
        _targetAllEnemies = true;
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }

        if (_targetAllEnemies)
        {
            Flash();
            await PrismWhirlwind.ExecuteIntentAll(
                new BlockingPlayerChoiceContext(),
                base.Owner,
                base.Amount,
                base.DynamicVars.Repeat.IntValue);
            await PowerCmd.Remove(this);
            return;
        }

        if (_target == null)
        {
            await PowerCmd.Remove(this);
            return;
        }

        Flash();
        await PrismWhirlwind.ExecuteIntent(
            new BlockingPlayerChoiceContext(),
            base.Owner,
            _target,
            base.Amount,
            base.DynamicVars.Repeat.IntValue);
        await PowerCmd.Remove(this);
    }
}
