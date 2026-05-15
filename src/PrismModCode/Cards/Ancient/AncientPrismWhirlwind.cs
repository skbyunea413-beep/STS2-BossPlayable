using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class AncientPrismWhirlwind : PrismCard
{
    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/ancientprismwhirlwind.png";

    public override IEnumerable<string> AllPortraitPaths => [CustomPortraitPath!];

    public AncientPrismWhirlwind() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new RepeatVar(3),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
    ];

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
        await WarningColorPower.TriggerForAttackIntent(ctx, base.Owner.Creature, null, true, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
