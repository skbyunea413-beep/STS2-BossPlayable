namespace PrismMod;

public sealed class Radiate : PrismCard
{
    protected override bool HasEnergyCostX => true;

    public Radiate() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    internal override bool IsBlockedByMissingRequiredPile =>
        !PrismRandomCardHelper.HasReclaimableCardInExhaust(base.Owner);

    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/radiate.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("BonusCards", 1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        int cardsToPlay = ResolveEnergyXValue() + base.DynamicVars["BonusCards"].IntValue;
        List<CardModel> exhaustCards = PrismRandomCardHelper.ReclaimableCardsInExhaust(base.Owner)
            .ToList();

        for (int i = 0; i < cardsToPlay && exhaustCards.Count > 0; i++)
        {
            var card = base.Owner.RunState.Rng.CombatCardGeneration.NextItem(exhaustCards);
            if (card == null)
            {
                break;
            }

            exhaustCards.Remove(card);
            await PrismRandomCardHelper.AutoPlayGeneratedCard(ctx, card);
        }
    }

    protected override void OnUpgrade() => base.DynamicVars["BonusCards"].UpgradeValueBy(2m);
}
