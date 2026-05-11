namespace PrismMod;

public sealed class MixedSignals : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/mixedsignals.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10m, ValueProp.Move)];

    public MixedSignals() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await PrismRandomCardHelper.AddRandomCardToHand(
            ctx,
            base.Owner,
            card => card.Type == CardType.Skill
                && PrismRandomCardHelper.IsOtherCharacterCard(card)
                && PrismRandomCardHelper.IsPlayableThisTurnAfterShard(base.Owner, card));
    }

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(4m);
}
