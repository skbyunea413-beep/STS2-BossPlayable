namespace PrismMod;

public sealed class SparkOfIntent : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/prismwhirlwind.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [PrismCardKeywords.AttackIntent];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];

    public SparkOfIntent() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(ctx);

        var intent = await PowerCmd.Apply<AttackIntentPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars.Damage.BaseValue,
            base.Owner.Creature,
            this);
        intent?.SetTarget(cardPlay.Target);
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(3m);
}
