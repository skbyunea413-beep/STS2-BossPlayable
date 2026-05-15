using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class LiterallyFemale : PrismCard
{
    private const string SpinSfx = "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_attack_spin";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new RepeatVar(3),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
    ];

    public LiterallyFemale() : base(3, CardType.Attack, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ExecuteRandomHits(
            ctx,
            base.Owner.Creature,
            base.DynamicVars.Damage.BaseValue,
            base.DynamicVars.Repeat.IntValue,
            this,
            playSpin: true);

        var intent = await PowerCmd.Apply<LiterallyFemaleIntentPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars.Damage.BaseValue,
            base.Owner.Creature,
            this);
        if (intent != null)
        {
            intent.Repeat = base.DynamicVars.Repeat.IntValue;
        }
    }

    internal static async Task ExecuteRandomHits(
        PlayerChoiceContext ctx,
        Creature owner,
        decimal damage,
        int repeat,
        CardModel source,
        bool playSpin)
    {
        var player = owner.Player;
        var combatState = owner.CombatState;
        if (player == null || combatState == null)
        {
            return;
        }

        for (int i = 0; i < repeat; i++)
        {
            var target = player.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
            if (target == null)
            {
                break;
            }

            var attack = DamageCmd.Attack(damage)
                .FromCard(source)
                .Targeting(target)
                .WithHitFx("vfx/vfx_attack_slash");

            if (playSpin && i == 0)
            {
                attack = attack
                    .WithAttackerAnim("AttackDouble", 0.2f, owner)
                    .OnlyPlayAnimOnce()
                    .WithAttackerFx(null, SpinSfx);
            }

            await attack.Execute(ctx);
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Repeat.UpgradeValueBy(1m);
}
