namespace PrismMod;

public sealed class AncientPrismWhirlwind : PrismCard
{
    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/prismwhirlwind.png";

    public override IEnumerable<string> AllPortraitPaths => [CustomPortraitPath!];

    public AncientPrismWhirlwind() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new RepeatVar(3),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [PrismCardKeywords.AttackIntent];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PrismWhirlwind.ExecuteIntentAll(
            ctx,
            base.Owner.Creature,
            base.DynamicVars.Damage.BaseValue,
            base.DynamicVars.Repeat.IntValue);

        var intent = await PowerCmd.Apply<AttackIntentPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars.Damage.BaseValue,
            base.Owner.Creature,
            this);
        intent?.SetTargetAllEnemies();
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
