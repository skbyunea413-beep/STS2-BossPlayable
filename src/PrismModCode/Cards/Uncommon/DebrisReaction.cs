namespace PrismMod;

public sealed class DebrisReaction : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/debrisreaction_edit.png";

    protected override bool ShouldGlowGoldInternal => HasExhaustedCard;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
        new BlockVar("BonusBlock", 7m, ValueProp.Move),
    ];

    public DebrisReaction() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if (!HasExhaustedCard)
        {
            return;
        }

        await CreatureCmd.GainBlock(
            base.Owner.Creature,
            base.DynamicVars["BonusBlock"].BaseValue,
            ValueProp.Move,
            cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
        base.DynamicVars["BonusBlock"].UpgradeValueBy(3m);
    }

    private bool HasExhaustedCard => PrismCardHistoryHelper.HasExhaustedCardThisTurn(base.Owner);
}
