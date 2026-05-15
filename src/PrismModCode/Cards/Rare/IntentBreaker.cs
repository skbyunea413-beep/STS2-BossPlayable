using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class IntentBreaker : PrismCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new RepeatVar(2),
        new DynamicVar("Percent", 25m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
    ];

    public IntentBreaker() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(base.DynamicVars.Repeat.IntValue)
            .WithAttackerAnim("AttackDouble", 0.2f, base.Owner.Creature)
            .OnlyPlayAnimOnce()
            .WithAttackerFx(null, PrismWhirlwind.SpinSfx)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(ctx);

        await AttackIntentPower.TriggerAllAgainstAndReduce(
            ctx,
            base.Owner,
            cardPlay.Target,
            base.DynamicVars["Percent"].BaseValue / 100m);
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(3m);
}
