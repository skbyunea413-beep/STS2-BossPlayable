namespace PrismMod;

public sealed class ChromaticAberration : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/chromaticaberration_EDIT.png";

    protected override bool ShouldGlowGoldInternal => PrismRandomCardHelper.HasOtherCharacterCardInHand(base.Owner);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new CardsVar(1),
    ];

    public ChromaticAberration() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if (!PrismRandomCardHelper.HasOtherCharacterCardInHand(base.Owner))
        {
            return;
        }

        await CardPileCmd.Draw(ctx, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(3m);
}
