namespace PrismMod;

public sealed class ArchmagesRune : PrismCard
{
    private const int MaxCardsToPlay = 30;

    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/archmagesrune.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("CostTotal", 10m)];

    public ArchmagesRune() : base(4, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        int targetCost = base.DynamicVars["CostTotal"].IntValue;
        int totalCost = 0;

        for (int playedCards = 0; playedCards < MaxCardsToPlay && totalCost < targetCost; playedCards++)
        {
            var randomCard = PrismRandomCardHelper.CreateRandomCardFromAllRunePools(base.Owner);
            if (randomCard == null)
            {
                return;
            }

            totalCost += PrismRandomCardHelper.GetGeneratedCardCost(randomCard);
            randomCard.AddKeyword(CardKeyword.Exhaust);
            await PrismRandomCardHelper.AutoPlayGeneratedCard(ctx, randomCard);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }

}
