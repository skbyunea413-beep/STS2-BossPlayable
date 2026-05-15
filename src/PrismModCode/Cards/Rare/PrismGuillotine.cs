namespace PrismMod;

public sealed class PrismGuillotine : PrismCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new RepeatVar(3),
        new DynamicVar("BonusDamage", 1m),
    ];

    public PrismGuillotine() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
        int played = PrismCardHistoryHelper.OtherCharacterCardsPlayedThisCombat(base.Owner);
        decimal damage = base.DynamicVars.Damage.BaseValue + played * base.DynamicVars["BonusDamage"].BaseValue;
        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(base.DynamicVars.Repeat.IntValue)
            .WithAttackerAnim("AttackDouble", 0.2f, base.Owner.Creature)
            .OnlyPlayAnimOnce()
            .WithAttackerFx(null, PrismWhirlwind.SpinSfx)
            .WithHitFx("vfx/vfx_attack_slash_heavy")
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
