namespace PrismMod;

public sealed class Buried : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/buried.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
        new CardsVar(1),
    ];

    public Buried() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            await PrismRandomCardHelper.AddRandomCardToHand(ctx, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
