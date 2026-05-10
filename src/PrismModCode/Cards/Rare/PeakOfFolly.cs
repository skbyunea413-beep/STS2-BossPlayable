using MegaCrit.Sts2.Core.Models.Cards;

namespace PrismMod;

public sealed class PeakOfFolly : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/peakoffolly.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(7),
        new DynamicVar("PlayedCards", 3m),
    ];

    public PeakOfFolly() : base(3, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        if (base.CombatState == null)
        {
            return;
        }

        List<CardModel> rocks = [];
        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            var rock = base.CombatState.CreateCard<GiantRock>(base.Owner);
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(rock);
            }

            rocks.Add(rock);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(rocks, PileType.Discard, base.Owner);

        for (int i = 0; i < base.DynamicVars["PlayedCards"].IntValue; i++)
        {
            await PrismRandomCardHelper.AutoPlayRandomCard(ctx, base.Owner, card => card.Rarity == CardRarity.Common);
        }
    }
}
