using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace PrismMod;

[HarmonyPatch(typeof(NCardLibraryGrid), nameof(NCardLibraryGrid.RefreshVisibility))]
internal static class PrismCardLibraryVisibilityPatch
{
    [HarmonyPostfix]
    private static void RevealPrismCards(
        HashSet<ModelId> ____seenCards,
        HashSet<CardModel> ____unlockedCards)
    {
        foreach (CardModel card in ModelDb.CardPool<PrismCardPool>().AllCards)
        {
            ____seenCards.Add(card.Id);
            ____unlockedCards.Add(card);
        }
    }
}
