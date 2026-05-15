using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using StsRadiate = MegaCrit.Sts2.Core.Models.Cards.Radiate;
using StsTactician = MegaCrit.Sts2.Core.Models.Cards.Tactician;

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
            .Where(card => !IsRecursiveRandomCard(card) &&
                !card.Keywords.Contains(CardKeyword.Unplayable) &&
                !card.Keywords.Contains(PrismCardKeywords.BattleStart));
    }

    private static bool IsRecursiveRandomCard(CardModel card)
    {
        return card is PrismWhirlwind or AncientPrismWhirlwind or RadiantGamble or AncientRadiantGamble or HiddenCard or ArchmagesRune
            or MixedSignals or PeakOfFolly or FieldProcurement
            or PrismaticCover or SecretProcurement
            or PrismRefraction or ShardSalvage or PrismRearrangement;
    }

    internal static bool IsOtherCharacterCard(CardModel card)
    {
        if (card is PrismCard ||
            card.Pool == ModelDb.CardPool<PrismCardPool>() ||
            card.VisualCardPool == ModelDb.CardPool<PrismCardPool>())
        {
            return false;
        }

        return ModelDb.AllCharacterCardPools.Contains(card.Pool) &&
            card.Pool != ModelDb.CardPool<PrismCardPool>();
    }

    internal static bool IsIroncladOrSilentCard(CardModel card)
    {
        return card.Pool == ModelDb.CardPool<IroncladCardPool>() ||
            card.Pool == ModelDb.CardPool<SilentCardPool>();
    }

    internal static bool IsIroncladOrSilentBasicNonStrikeDefend(CardModel card)
    {
        return card.Rarity == CardRarity.Basic &&
            IsIroncladOrSilentCard(card) &&
            !card.Tags.Contains(CardTag.Strike) &&
            !card.Tags.Contains(CardTag.Defend);
    }

    internal static bool IsPlayableThisTurnAfterShard(Player player, CardModel card)
    {
        if (card.EnergyCost.CostsX)
        {
            return false;
        }

        int cost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        if (player.Relics.OfType<PrismaticShard>().Any() && IsOtherCharacterCard(card))
        {
            cost = System.Math.Max(0, cost - 1);
        }

        int stars = player.PlayerCombatState?.Stars ?? 0;
        if (card.HasStarCostX && stars <= 0)
        {
            return false;
        }

        int starCost = card.HasStarCostX ? stars : card.GetStarCostWithModifiers();
        return cost <= player.GetEnergy() &&
            starCost <= stars;
    }

    internal static bool CanPayForCardNow(Player player, CardModel card)
    {
        if (card.EnergyCost.CostsX)
        {
            return false;
        }

        int cost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        int stars = player.PlayerCombatState?.Stars ?? 0;
        int starCost = card.HasStarCostX ? stars : card.GetStarCostWithModifiers();
        return cost <= player.GetEnergy() &&
            starCost <= stars;
    }

    internal static bool IsUsablePileCard(CardModel card)
    {
        return card.Type is not (CardType.Curse or CardType.Status) &&
            !card.Keywords.Contains(CardKeyword.Unplayable);
    }

    internal static IEnumerable<CardModel> UsableCardsInPile(Player player, PileType pileType)
    {
        return pileType.GetPile(player).Cards.Where(IsUsablePileCard);
    }

    internal static IEnumerable<CardModel> ReclaimableCardsInExhaust(Player player)
    {
        return UsableCardsInPile(player, PileType.Exhaust)
            .Where(card => GetGeneratedCardCost(card) > 0);
    }

    internal static bool HasUsableCardInPile(Player player, PileType pileType)
    {
        return UsableCardsInPile(player, pileType).Any();
    }

    internal static bool HasReclaimableCardInExhaust(Player player)
    {
        return ReclaimableCardsInExhaust(player).Any();
    }

    internal static IEnumerable<CardModel> OtherCharacterCardOptions(Player player)
    {
        return AllRandomCardOptions(player)
            .Where(IsOtherCharacterCard)
            .Where(IsAllowedOtherCharacterCard);
    }

    private static bool IsAllowedOtherCharacterCard(CardModel card)
    {
        if (card.MultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly)
        {
            return false;
        }

        if (card is GrandFinale or KnifeTrap)
        {
            return false;
        }

        if (card is HiddenCache or Convergence or Outmaneuver or Invoke or Scavenge or Prolong)
        {
            return false;
        }

        if (card is StsRadiate or StsTactician)
        {
            return false;
        }

        if (card is Sacrifice or FocusedStrike or Hotfix or Synchronize)
        {
            return false;
        }

        if (card.Type == CardType.Power && card is not StoneArmor)
        {
            return false;
        }

        if (card.Tags.Contains(CardTag.OstyAttack))
        {
            return false;
        }

        if (card.Pool == ModelDb.CardPool<DefectCardPool>() &&
            card is not Shatter &&
            card.OrbEvokeType != OrbEvokeType.None)
        {
            return false;
        }

        return true;
    }

    internal static IEnumerable<CardModel> PlayableOtherCharacterCardOptions(
        Player player,
        Func<CardModel, bool>? filter = null)
    {
        IEnumerable<CardModel> options = OtherCharacterCardOptions(player)
            .Where(card => IsPlayableThisTurnAfterShard(player, card));

        if (filter != null)
        {
            options = options.Where(filter);
        }

        return options;
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

    internal static CardModel? CreatePlayableOtherCharacterCard(Player player, Func<CardModel, bool>? filter = null)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return null;
        }

        return CardFactory
            .GetDistinctForCombat(
                player,
                PlayableOtherCharacterCardOptions(player, filter),
                1,
                player.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
    }

    internal static CardModel? CreateOtherCharacterCard(Player player, Func<CardModel, bool>? filter = null)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return null;
        }

        IEnumerable<CardModel> options = OtherCharacterCardOptions(player);
        if (filter != null)
        {
            options = options.Where(filter);
        }

        return CardFactory
            .GetDistinctForCombat(
                player,
                options,
                1,
                player.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
    }

    internal static IReadOnlyList<CardModel> CreatePlayableOtherCharacterChoices(
        Player player,
        int count,
        Func<CardModel, bool>? filter = null)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return [];
        }

        return CardFactory
            .GetDistinctForCombat(
                player,
                PlayableOtherCharacterCardOptions(player, filter),
                count,
                player.RunState.Rng.CombatCardGeneration)
            .ToList();
    }

    internal static IReadOnlyList<CardModel> CreateOtherCharacterChoices(
        Player player,
        int count,
        Func<CardModel, bool>? filter = null)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return [];
        }

        IEnumerable<CardModel> options = OtherCharacterCardOptions(player);
        if (filter != null)
        {
            options = options.Where(filter);
        }

        return CardFactory
            .GetDistinctForCombat(
                player,
                options,
                count,
                player.RunState.Rng.CombatCardGeneration)
            .ToList();
    }

    internal static CardModel? CreateRandomCardIncludingBasic(Player player, Func<CardModel, bool>? filter = null)
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

        var canonicalCard = player.RunState.Rng.CombatCardGeneration.NextItem(options.Distinct().ToList());
        return canonicalCard == null ? null : combatState.CreateCard(canonicalCard, player);
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

        player.Relics.OfType<PrismaticShard>().FirstOrDefault()?.ApplyGeneratedCardModifiers(card);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        return card;
    }

    internal static async Task<CardModel?> AddPlayableOtherCharacterCardToHand(
        PlayerChoiceContext ctx,
        Player player,
        Func<CardModel, bool>? filter = null)
    {
        var card = CreatePlayableOtherCharacterCard(player, filter);
        if (card == null)
        {
            return null;
        }

        player.Relics.OfType<PrismaticShard>().FirstOrDefault()?.ApplyGeneratedCardModifiers(card);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        return card;
    }

    internal static async Task<CardModel?> AddOtherCharacterCardToHand(
        PlayerChoiceContext ctx,
        Player player,
        Func<CardModel, bool>? filter = null)
    {
        var card = CreateOtherCharacterCard(player, filter);
        if (card == null)
        {
            return null;
        }

        player.Relics.OfType<PrismaticShard>().FirstOrDefault()?.ApplyGeneratedCardModifiers(card);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        return card;
    }

    internal static async Task<CardModel?> ChoosePlayableOtherCharacterCardToHand(
        PlayerChoiceContext ctx,
        Player player,
        int choices,
        Func<CardModel, bool>? filter = null)
    {
        var cards = CreatePlayableOtherCharacterChoices(player, choices, filter);
        foreach (var card in cards)
        {
            player.Relics.OfType<PrismaticShard>().FirstOrDefault()?.ApplyGeneratedCardModifiers(card);
        }

        var selected = await CardSelectCmd.FromChooseACardScreen(ctx, cards, player);
        if (selected == null)
        {
            return null;
        }

        await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, player);
        return selected;
    }

    internal static async Task<CardModel?> ChooseOtherCharacterCardToHand(
        PlayerChoiceContext ctx,
        Player player,
        int choices,
        Func<CardModel, bool>? filter = null)
    {
        var cards = CreateOtherCharacterChoices(player, choices, filter);
        foreach (var card in cards)
        {
            player.Relics.OfType<PrismaticShard>().FirstOrDefault()?.ApplyGeneratedCardModifiers(card);
        }

        var selected = await CardSelectCmd.FromChooseACardScreen(ctx, cards, player);
        if (selected == null)
        {
            return null;
        }

        await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, player);
        return selected;
    }

    internal static async Task<CardModel?> AddRandomCardToHandIncludingBasic(PlayerChoiceContext ctx, Player player, Func<CardModel, bool>? filter = null)
    {
        var card = CreateRandomCardIncludingBasic(player, filter);
        if (card == null)
        {
            return null;
        }

        player.Relics.OfType<PrismaticShard>().FirstOrDefault()?.ApplyGeneratedCardModifiers(card);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        return card;
    }

    internal static async Task<CardModel?> AddIroncladOrSilentBasicCardToHand(
        PlayerChoiceContext ctx,
        Player player,
        bool upgraded)
    {
        var card = CreateRandomCardIncludingBasic(player, IsIroncladOrSilentBasicNonStrikeDefend);
        if (card == null)
        {
            return null;
        }

        if (upgraded)
        {
            CardCmd.Upgrade(card, CardPreviewStyle.None);
        }

        player.Relics.OfType<PrismaticShard>().FirstOrDefault()?.ApplyGeneratedCardModifiers(card);
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

    internal static bool HasCostAtLeast(CardModel card, int cost)
    {
        return !card.EnergyCost.CostsX &&
            card.EnergyCost.GetWithModifiers(CostModifiers.All) >= cost;
    }

    internal static bool HasHandCardWithCostAtLeast(Player player, int cost)
    {
        return PileType.Hand.GetPile(player).Cards
            .Any(card => HasCostAtLeast(card, cost));
    }

    internal static bool HasOtherCharacterCardInHand(Player player)
    {
        return PileType.Hand.GetPile(player).Cards
            .Any(IsOtherCharacterCard);
    }

    internal static CardModel? ReduceRandomHandCardCostThisTurn(
        Player player,
        Func<CardModel, bool> filter)
    {
        var cards = PileType.Hand.GetPile(player).Cards
            .Where(card => !card.EnergyCost.CostsX && filter(card))
            .ToList();
        var card = player.RunState.Rng.CombatCardSelection.NextItem(cards);
        if (card == null)
        {
            return null;
        }

        card.EnergyCost.AddThisTurn(-1, reduceOnly: true);
        return card;
    }

    internal static CardModel? SetRandomHandCardCostThisTurn(
        Player player,
        int cost,
        Func<CardModel, bool> filter)
    {
        var cards = PileType.Hand.GetPile(player).Cards
            .Where(card => !card.EnergyCost.CostsX && filter(card))
            .ToList();
        var card = player.RunState.Rng.CombatCardSelection.NextItem(cards);
        if (card == null)
        {
            return null;
        }

        card.EnergyCost.SetThisTurn(cost, reduceOnly: true);
        return card;
    }
}
