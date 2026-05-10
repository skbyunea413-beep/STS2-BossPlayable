using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace PrismMod;

internal static class PrismRandomCardHelper
{
    private static IEnumerable<CardModel> AllRandomCardOptions(Player player)
    {
        return ModelDb.AllCharacterCardPools
            .Concat([
                ModelDb.CardPool<ColorlessCardPool>(),
                ModelDb.CardPool<PrismCardPool>()
            ])
            .Distinct()
            .SelectMany(pool => pool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(card => !IsRecursiveRandomCard(card) && !card.Keywords.Contains(CardKeyword.Unplayable));
    }

    private static bool IsRecursiveRandomCard(CardModel card)
    {
        return card is PrismWhirlwind or AncientPrismWhirlwind or RadiantGamble or HiddenCard or ArchmagesRune
            or MixedSignals or BorrowedFangs or PeakOfFolly;
    }

    internal static CardModel? CreateRandomCard(Player player, Func<CardModel, bool>? filter = null)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return null;
        }

        IEnumerable<CardModel> options = AllRandomCardOptions(player);

        if (filter != null)
        {
            options = options.Where(filter);
        }

        return CardFactory
            .GetDistinctForCombat(player, options, 1, player.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
    }

    internal static CardModel? CreateRandomCardFromAllRunePools(Player player, Func<CardModel, bool>? filter = null)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return null;
        }

        IEnumerable<CardModel> options = AllRandomCardOptions(player)
            .Where(card => card is not ArchmagesRune);

        if (filter != null)
        {
            options = options.Where(filter);
        }

        return CardFactory
            .GetForCombat(player, options, 1, player.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
    }

    internal static async Task<CardModel?> AddRandomCardToHand(PlayerChoiceContext ctx, Player player, Func<CardModel, bool>? filter = null)
    {
        var card = CreateRandomCard(player, filter);
        if (card == null)
        {
            return null;
        }

        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        return card;
    }

    internal static async Task<CardModel?> AutoPlayRandomCard(PlayerChoiceContext ctx, Player player, Func<CardModel, bool>? filter = null)
    {
        var card = CreateRandomCard(player, filter);
        if (card == null)
        {
            return null;
        }

        await AutoPlayGeneratedCard(ctx, card);
        return card;
    }

    internal static Task AutoPlayGeneratedCard(PlayerChoiceContext ctx, CardModel card)
    {
        if (card.EnergyCost.CostsX)
        {
            card.EnergyCost.CapturedXValue = card.Owner.PlayerCombatState?.MaxEnergy ?? card.Owner.MaxEnergy;
        }

        return CardCmd.AutoPlay(ctx, card, null, skipXCapture: true);
    }

    internal static int GetGeneratedCardCost(CardModel card)
    {
        if (card.EnergyCost.CostsX)
        {
            return card.Owner.PlayerCombatState?.MaxEnergy ?? card.Owner.MaxEnergy;
        }

        return card.EnergyCost.GetWithModifiers(CostModifiers.All);
    }
}
