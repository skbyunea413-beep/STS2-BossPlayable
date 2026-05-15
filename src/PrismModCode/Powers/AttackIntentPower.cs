using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class AttackIntentPower : PrismPower
{
    private const int Repeat = 1;
    private Creature? _target;
    private bool _targetAllEnemies;
    private bool _hasResolved;
    private int _delayTurns;

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

    internal static async Task IncreaseAll(PlayerChoiceContext ctx, Player player, decimal amount, CardModel? source)
    {
        var powers = player.Creature.GetPowerInstances<AttackIntentPower>().ToList();
        foreach (var power in powers)
        {
            await PowerCmd.ModifyAmount(ctx, power, amount, player.Creature, source);
        }
    }

    internal static async Task DelayAndDoubleAll(PlayerChoiceContext ctx, Player player, CardModel? source)
    {
        var powers = player.Creature.GetPowerInstances<AttackIntentPower>()
            .Where(power => !power._hasResolved)
            .ToList();
        foreach (var power in powers)
        {
            power._delayTurns++;
            await PowerCmd.ModifyAmount(ctx, power, power.Amount, player.Creature, source);
        }
    }

    internal static async Task TriggerAllAgainstAndReduce(PlayerChoiceContext ctx, Player player, Creature target, decimal reduceRatio)
    {
        var powers = player.Creature.GetPowerInstances<AttackIntentPower>()
            .Where(power => !power._hasResolved)
            .ToList();

        foreach (var power in powers)
        {
            power.Flash();
            await PrismWhirlwind.ExecuteIntent(ctx, power.Owner, target, power.Amount, power.DynamicVars.Repeat.IntValue);

            decimal reduction = System.Math.Ceiling(power.Amount * reduceRatio);
            if (reduction >= power.Amount)
            {
                await PowerCmd.Remove(power);
            }
            else
            {
                await PowerCmd.ModifyAmount(ctx, power, -reduction, player.Creature, null);
            }
        }
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }

        if (_hasResolved)
        {
            return;
        }

        if (_delayTurns > 0)
        {
            _delayTurns--;
            Flash();
            return;
        }

        if (_targetAllEnemies)
        {
            var allEnemyIntents = base.Owner.GetPowerInstances<AttackIntentPower>()
                .Where(power => power._targetAllEnemies && !power._hasResolved && power._delayTurns <= 0)
                .ToList();
            decimal totalDamage = allEnemyIntents.Sum(power => power.Amount);

            foreach (var power in allEnemyIntents)
            {
                power._hasResolved = true;
            }

            foreach (var power in allEnemyIntents)
            {
                power.Flash();
            }

            await PrismWhirlwind.ExecuteIntentAll(
                new BlockingPlayerChoiceContext(),
                base.Owner,
                totalDamage,
                base.DynamicVars.Repeat.IntValue);

            foreach (var power in allEnemyIntents)
            {
                await PowerCmd.Remove(power);
            }
            return;
        }

        if (_target == null)
        {
            _hasResolved = true;
            await PowerCmd.Remove(this);
            return;
        }

        _hasResolved = true;
        Flash();
        if (_target == base.Owner)
        {
            await CreatureCmd.Damage(
                new BlockingPlayerChoiceContext(),
                base.Owner,
                base.Amount,
                ValueProp.Move,
                base.Owner,
                null);
            await PowerCmd.Remove(this);
            return;
        }

        await PrismWhirlwind.ExecuteIntent(
            new BlockingPlayerChoiceContext(),
            base.Owner,
            _target,
            base.Amount,
            base.DynamicVars.Repeat.IntValue);
        await PowerCmd.Remove(this);
    }
}
