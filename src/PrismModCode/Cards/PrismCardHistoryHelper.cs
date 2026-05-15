using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;

namespace PrismMod;

internal static class PrismCardHistoryHelper
{
    internal static bool HasGeneratedCardThisTurn(Player player)
    {
        var combatState = player.Creature.CombatState;
        return CombatManager.Instance.History.Entries
            .OfType<CardGeneratedEntry>()
            .Any(entry => entry.HappenedThisTurn(combatState) &&
                (entry.Creator == player || entry.Card.Owner == player));
    }

    internal static bool HasExhaustedCardThisTurn(Player player)
    {
        var combatState = player.Creature.CombatState;
        return CombatManager.Instance.History.Entries
            .OfType<CardExhaustedEntry>()
            .Any(entry => entry.HappenedThisTurn(combatState) && entry.Card.Owner == player);
    }

    internal static bool HasOtherCharacterCardInHand(Player player)
    {
        return PileType.Hand.GetPile(player).Cards
            .Any(PrismRandomCardHelper.IsOtherCharacterCard);
    }

    internal static bool HasPlayedCostAtLeastThisTurn(Player player, int cost)
    {
        var combatState = player.Creature.CombatState;
        return CombatManager.Instance.History.CardPlaysFinished
            .Any(entry => entry.HappenedThisTurn(combatState) &&
                entry.CardPlay.Card.Owner == player &&
                !entry.CardPlay.IsAutoPlay &&
                PrismRandomCardHelper.HasCostAtLeast(entry.CardPlay.Card, cost));
    }

    internal static int OtherCharacterCardsPlayedThisCombat(Player player)
    {
        return CombatManager.Instance.History.CardPlaysFinished
            .Count(entry => entry.CardPlay.Card.Owner == player &&
                !entry.CardPlay.IsAutoPlay &&
                PrismRandomCardHelper.IsOtherCharacterCard(entry.CardPlay.Card));
    }

    internal static bool HasPlayedOtherCharacterCardThisTurn(Player player)
    {
        var combatState = player.Creature.CombatState;
        return CombatManager.Instance.History.CardPlaysFinished
            .Any(entry => entry.HappenedThisTurn(combatState) &&
                entry.CardPlay.Card.Owner == player &&
                !entry.CardPlay.IsAutoPlay &&
                PrismRandomCardHelper.IsOtherCharacterCard(entry.CardPlay.Card));
    }

    internal static bool HasAttackIntent(Player player)
    {
        return player.Creature.GetPower<AttackIntentPower>() != null ||
            player.Creature.GetPower<AttackIntentWeakPower>() != null;
    }
}
