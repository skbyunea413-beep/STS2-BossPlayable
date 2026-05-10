namespace PrismMod;

public sealed class RadiantGamble : PrismCard
{
    private const int MaxCardsToPlay = 20;

    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/radiantgamble.png";

    public override IEnumerable<string> AllPortraitPaths => [CustomPortraitPath!];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("CostTotal", 4m)];

    public RadiantGamble() : base(3, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        int targetCost = base.DynamicVars["CostTotal"].IntValue;
        int totalCost = 0;

        for (int playedCards = 0; playedCards < MaxCardsToPlay && totalCost < targetCost; playedCards++)
        {
            var randomCard = PrismRandomCardHelper.CreateRandomCard(base.Owner);
            if (randomCard == null)
            {
                return;
            }

            totalCost += PrismRandomCardHelper.GetGeneratedCardCost(randomCard);
            randomCard.AddKeyword(CardKeyword.Exhaust);
            await PrismRandomCardHelper.AutoPlayGeneratedCard(ctx, randomCard);
        }
    }

    protected override void OnUpgrade() => base.DynamicVars["CostTotal"].UpgradeValueBy(2m);
}
