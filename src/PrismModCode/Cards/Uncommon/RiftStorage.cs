namespace PrismMod;

public sealed class RiftStorage : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/riftstorage.png";

    internal override bool IsBlockedByMissingRequiredPile =>
        !PrismRandomCardHelper.ReclaimableCardsInExhaust(base.Owner)
            .Any(card => card.Type == CardType.Skill);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public RiftStorage() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = PrismRandomCardHelper.ReclaimableCardsInExhaust(base.Owner)
            .Where(card => card.Type == CardType.Skill)
            .ToList();
        List<CardModel> selected = [];
        for (int i = 0; i < base.DynamicVars.Cards.IntValue && cards.Count > 0; i++)
        {
            var card = base.Owner.RunState.Rng.CombatCardSelection.NextItem(cards);
            if (card == null)
            {
                break;
            }

            cards.Remove(card);
            selected.Add(card);
        }

        await CardPileCmd.Add(selected, PileType.Hand);
    }

    protected override void OnUpgrade() => base.DynamicVars.Cards.UpgradeValueBy(1m);
}
