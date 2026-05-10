using MegaCrit.Sts2.Core.Models.Cards;

namespace PrismMod;

public sealed class PeakOfFolly : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/peakoffolly.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(4),
        new DynamicVar("Rocks", 2m),
    ];

    public PeakOfFolly() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        if (base.CombatState == null)
        {
            return;
        }

        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            await PrismRandomCardHelper.AddRandomCardToHand(ctx, base.Owner, IsOtherCharacterCommonCard);
        }

        List<CardModel> rocks = [];
        for (int i = 0; i < base.DynamicVars["Rocks"].IntValue; i++)
        {
            var rock = base.CombatState.CreateCard<GiantRock>(base.Owner);
            rocks.Add(rock);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(rocks, PileType.Discard, base.Owner);
    }

    protected override void OnUpgrade() => base.DynamicVars["Rocks"].UpgradeValueBy(-1m);

    private static bool IsOtherCharacterCommonCard(CardModel card)
    {
        return card.Rarity == CardRarity.Common
            && ModelDb.AllCharacterCardPools.Contains(card.Pool);
    }
}
