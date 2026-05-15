namespace PrismMod;

public sealed class PrismaticCover : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/prismaticcover.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9m, ValueProp.Move),
    ];

    public PrismaticCover() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        foreach (var card in PileType.Hand.GetPile(base.Owner).Cards.Where(PrismRandomCardHelper.IsOtherCharacterCard))
        {
            if (!card.EnergyCost.CostsX)
            {
                card.EnergyCost.AddThisTurn(-1, reduceOnly: true);
            }
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(3m);
}
