using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class PrismRearrangement : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/prismrearrangement.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    public PrismRearrangement() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var selected = (await CardSelectCmd.FromHand(
            ctx,
            base.Owner,
            new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1),
            card => PrismRandomCardHelper.IsUsablePileCard(card) &&
                PrismRandomCardHelper.IsOtherCharacterCard(card) &&
                card.IsTransformable,
            this)).FirstOrDefault();
        if (selected == null)
        {
            return;
        }

        var replacement = PrismRandomCardHelper.CreateOtherCharacterCard(base.Owner);
        if (replacement == null)
        {
            return;
        }

        base.Owner.Relics.OfType<PrismaticShard>().FirstOrDefault()?.ApplyGeneratedCardModifiers(replacement);
        await CardCmd.Transform(selected, replacement);
        for (int i = 1; i < base.DynamicVars.Cards.IntValue; i++)
        {
            await PrismRandomCardHelper.AddOtherCharacterCardToHand(ctx, base.Owner);
        }
    }

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}
