namespace PrismMod;

public sealed class WarningColorPower : PrismPower
{
    private bool _triggeredThisTurn;

    internal decimal Damage { get; set; }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("WeakPower", 1m),
        new DamageVar(0m, ValueProp.Move),
    ];

    public override Task AfterEnergyReset(Player player)
    {
        if (player == base.Owner.Player)
        {
            _triggeredThisTurn = false;
        }

        return Task.CompletedTask;
    }

    internal static async Task TriggerForAttackIntent(
        PlayerChoiceContext choiceContext,
        Creature owner,
        Creature? target,
        bool allEnemies,
        CardModel? sourceCard)
    {
        var power = owner.GetPower<WarningColorPower>();
        if (power == null || power._triggeredThisTurn)
        {
            return;
        }

        var targets = allEnemies
            ? owner.CombatState?.HittableEnemies.ToList() ?? []
            : target == null ? [] : [target];
        if (targets.Count == 0)
        {
            return;
        }

        power._triggeredThisTurn = true;
        power.Flash();

        foreach (var enemy in targets)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, enemy, power.Amount, owner, sourceCard);

            if (power.Damage > 0 && sourceCard != null)
            {
                await DamageCmd.Attack(power.Damage)
                    .FromCard(sourceCard)
                    .Targeting(enemy)
                    .WithHitFx("vfx/vfx_starry_impact")
                    .Execute(choiceContext);
            }
        }
    }
}
