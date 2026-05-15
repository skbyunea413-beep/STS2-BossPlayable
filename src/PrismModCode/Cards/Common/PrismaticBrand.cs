using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class PrismaticBrand : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/prismaticbrand.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new PowerVar<WeakPower>(1m),
    ];

    public PrismaticBrand() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithAttackerAnim("AttackDouble", 0.2f, base.Owner.Creature)
            .OnlyPlayAnimOnce()
            .WithAttackerFx(null, PrismWhirlwind.SpinSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);

        await PowerCmd.Apply<WeakPower>(
            ctx,
            cardPlay.Target,
            base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars.Weak.UpgradeValueBy(1m);
    }
}
