using MegaCrit.Sts2.Core.CardSelection;

namespace PrismMod;

public sealed class BorrowedMoment : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/borrowedmoment.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new DynamicVar("Draw", 1m),
    ];

    public BorrowedMoment() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, base.DynamicVars["Draw"].IntValue, base.Owner);

        var selected = await CardSelectCmd.FromHand(
            ctx,
            base.Owner,
            new CardSelectorPrefs(base.SelectionScreenPrompt, 0, base.DynamicVars.Cards.IntValue),
            card => !card.ShouldRetainThisTurn,
            this);

        foreach (var card in selected)
        {
            card.GiveSingleTurnRetain();
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Cards.UpgradeValueBy(1m);
}
