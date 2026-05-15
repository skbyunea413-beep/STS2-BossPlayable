namespace PrismMod;

public sealed class RecycledGuard : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/recycledguard.png";

    protected override bool ShouldGlowGoldInternal => CanRecycle;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10m, ValueProp.Move),
        new CardsVar(1),
    ];

    public RecycledGuard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if (!CanRecycle)
        {
            return;
        }

        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            var card = base.Owner.RunState.Rng.CombatCardSelection.NextItem(
                PrismRandomCardHelper.ReclaimableCardsInExhaust(base.Owner).ToList());
            if (card == null)
            {
                return;
            }

            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(3m);

    private bool CanRecycle => PrismRandomCardHelper.HasReclaimableCardInExhaust(base.Owner);
}
