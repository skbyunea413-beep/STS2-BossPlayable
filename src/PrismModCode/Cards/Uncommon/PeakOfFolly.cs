using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;

namespace PrismMod;

public sealed class PeakOfFolly : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/peakoffolly.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<GiantRock>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new DynamicVar("Rocks", 2m),
    ];

    public PeakOfFolly() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        if (base.CombatState == null)
        {
            return;
        }

        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            await PrismRandomCardHelper.AddOtherCharacterCardToHand(
                ctx,
                base.Owner,
                card => card.Rarity == CardRarity.Common);
        }

        List<CardModel> rocks = [];
        for (int i = 0; i < base.DynamicVars["Rocks"].IntValue; i++)
        {
            var rock = base.CombatState.CreateCard<GiantRock>(base.Owner);
            rocks.Add(rock);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(rocks, PileType.Discard, base.Owner);
    }

    protected override void OnUpgrade() => base.DynamicVars.Cards.UpgradeValueBy(1m);
}
