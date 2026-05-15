using MegaCrit.Sts2.Core.Models.Cards;

namespace PrismMod;

public sealed class PrismRefraction : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/prismrefraction.png";

    public PrismRefraction() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var transformations = PileType.Hand.GetPile(base.Owner).Cards
            .Where(card => card.IsTransformable)
            .Select(CreateOtherCharacterTransformation)
            .Where(transformation => transformation.HasValue)
            .Select(transformation => transformation!.Value)
            .ToList();

        await CardCmd.Transform(transformations, null);
    }

    private CardTransformation? CreateOtherCharacterTransformation(CardModel original)
    {
        var replacement = PrismRandomCardHelper.CreateOtherCharacterCard(base.Owner);
        if (replacement == null)
        {
            return null;
        }

        base.Owner.Relics.OfType<PrismaticShard>().FirstOrDefault()?.ApplyGeneratedCardModifiers(replacement);
        return new CardTransformation(original, replacement);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
