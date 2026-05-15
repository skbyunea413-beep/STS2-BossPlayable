using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class DelayedAmplification : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/delayedamplification.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
    ];

    public DelayedAmplification() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await AttackIntentPower.DelayAndDoubleAll(ctx, base.Owner, this);
    }

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(5m);
}
