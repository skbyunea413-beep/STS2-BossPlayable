using MegaCrit.Sts2.Core.CardSelection;

namespace PrismMod;

public sealed class CommonRummage : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/commonrummage.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new DynamicVar("BonusCards", 1m),
        new DynamicVar("Discard", 1m),
    ];

    public CommonRummage() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override bool ShouldGlowGoldInternal => HasOtherCharacterCardInHand;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, base.DynamicVars.Cards.BaseValue, base.Owner);
        if (HasOtherCharacterCardInHand)
        {
            await CardPileCmd.Draw(ctx, base.DynamicVars["BonusCards"].BaseValue, base.Owner);
        }

        if (PileType.Hand.GetPile(base.Owner).Cards.Count == 0)
        {
            return;
        }

        var selected = await CardSelectCmd.FromHandForDiscard(
            ctx,
            base.Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, base.DynamicVars["Discard"].IntValue),
            null,
            this);
        await CardCmd.Discard(ctx, selected);
    }

    protected override void OnUpgrade() => base.EnergyCost.UpgradeBy(-1);

    private bool HasOtherCharacterCardInHand => PrismRandomCardHelper.HasOtherCharacterCardInHand(base.Owner);
}
