using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class PrismWhirlwind : PrismCard
{
    internal const string SpinSfx = "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_attack_spin";

    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/prismwhirlwind.png";

    public PrismWhirlwind() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new RepeatVar(3),
        new PowerVar<WeakPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (cardPlay.Target == null)
        {
            return;
        }

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(base.DynamicVars.Repeat.IntValue)
            .WithAttackerAnim("AttackDouble", 0.2f, base.Owner.Creature)
            .OnlyPlayAnimOnce()
            .WithAttackerFx(null, SpinSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);

        var intent = await PowerCmd.Apply<AttackIntentWeakPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature,
            this);
        intent?.SetTarget(cardPlay.Target);
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(1m);

    internal static Task ExecuteIntent(PlayerChoiceContext ctx, Creature owner, Creature target, decimal damage, int repeat)
    {
        var player = owner.Player;
        var combatState = owner.CombatState;
        if (player == null || combatState == null || !target.IsAlive || !combatState.HittableEnemies.Contains(target))
        {
            return Task.CompletedTask;
        }

        return DamageCmd.Attack(damage)
            .FromCard(player.RunState.CreateCard<PrismWhirlwind>(player))
            .Targeting(target)
            .WithHitCount(repeat)
            .WithAttackerAnim("AttackDouble", 0.2f, owner)
            .OnlyPlayAnimOnce()
            .WithAttackerFx(null, SpinSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }

    internal static Task ExecuteIntentAll(PlayerChoiceContext ctx, Creature owner, decimal damage, int repeat)
    {
        var player = owner.Player;
        var combatState = owner.CombatState;
        if (player == null || combatState == null)
        {
            return Task.CompletedTask;
        }

        return DamageCmd.Attack(damage)
            .FromCard(player.RunState.CreateCard<PrismWhirlwind>(player))
            .TargetingAllOpponents(combatState)
            .WithHitCount(repeat)
            .WithAttackerAnim("AttackDouble", 0.2f, owner)
            .OnlyPlayAnimOnce()
            .WithAttackerFx(null, SpinSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }
}
