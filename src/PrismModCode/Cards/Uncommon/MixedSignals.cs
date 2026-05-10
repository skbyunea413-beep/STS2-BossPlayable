namespace PrismMod;

public sealed class MixedSignals : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/radiantgamble.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10m, ValueProp.Move)];

    public MixedSignals() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await PrismRandomCardHelper.AutoPlayRandomCard(ctx, base.Owner);
    }

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(4m);
}
