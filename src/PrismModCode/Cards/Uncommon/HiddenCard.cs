namespace PrismMod;

public sealed class HiddenCard : PrismCard
{
    private const int MaxCardsToPlay = 20;

    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/hiddencard.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("CostTotal", 4m)];

    public HiddenCard() : base(2, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        int targetCost = base.DynamicVars["CostTotal"].IntValue;
        int totalCost = 0;

        for (int playedCards = 0; playedCards < MaxCardsToPlay && totalCost < targetCost; playedCards++)
        {
            var randomCard = PrismRandomCardHelper.CreateRandomCard(base.Owner, IsEligibleHiddenCard);
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
        base.DynamicVars["CostTotal"].UpgradeValueBy(2m);
    }

    private static bool IsEligibleHiddenCard(CardModel card)
    {
        return !card.EnergyCost.CostsX &&
            card.EnergyCost.GetWithModifiers(CostModifiers.All) >= 2;
    }

}
