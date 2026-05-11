namespace PrismMod;

public sealed class PrismaticCover : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/guard.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
        new CardsVar(1),
    ];

    public PrismaticCover() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

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

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(3m);
}
