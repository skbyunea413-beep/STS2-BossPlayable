namespace PrismMod;

public sealed class RecycledGuard : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/recycledguard.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new CardsVar(1),
    ];

    public RecycledGuard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if (PileType.Exhaust.GetPile(base.Owner).Cards.Count == 0)
        {
            return;
        }

        await PrismRandomCardHelper.AddRandomCardToHand(
            ctx,
            base.Owner,
            card => PrismRandomCardHelper.IsOtherCharacterCard(card)
                && PrismRandomCardHelper.IsPlayableThisTurnAfterShard(base.Owner, card));
    }

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(3m);
}
