using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace PrismMod;

[HarmonyPatch(typeof(Orobas), "get_SeaGlassOptions")]
internal static class SeaGlassOptionsPatch
{
    [HarmonyPostfix]
    private static void ExcludePrismSeaGlassOption(ref IEnumerable<EventOption> __result)
    {
        __result = __result.Where(IsNotPrismSeaGlass);
    }

    internal static bool IsNotPrismSeaGlass(EventOption option) =>
        option.Relic is not SeaGlass seaGlass ||
        seaGlass.CharacterId != ModelDb.Character<PrismCharacter>().Id;
}

[HarmonyPatch(typeof(Orobas), "GenerateInitialOptions")]
internal static class OrobasInitialOptionsPatch
{
    [HarmonyPostfix]
    private static void ExcludePrismSeaGlassOption(ref IReadOnlyList<EventOption> __result)
    {
        __result = __result.Where(SeaGlassOptionsPatch.IsNotPrismSeaGlass).ToArray();
    }
}

[HarmonyPatch(typeof(CardCreationOptions), nameof(CardCreationOptions.GetPossibleCards))]
internal static class NonPrismCardCreationPatch
{
    [HarmonyPostfix]
    private static void ExcludePrismCardsForOtherCharacters(Player player, ref IEnumerable<CardModel> __result)
    {
        if (player.Character is PrismCharacter)
        {
            return;
        }

        __result = __result.Where(card => card is not PrismCard);
    }
}

[HarmonyPatch(typeof(Kaleidoscope), nameof(Kaleidoscope.IsAllowedAtNeow))]
internal static class KaleidoscopeAllowedAtNeowPatch
{
    [HarmonyPrefix]
    private static bool IgnorePrismForUnlockRequirement(Player player, ref bool __result)
    {
        int unlockedNonPrismPools = player.UnlockState.CharacterCardPools
            .Count(pool => pool is not PrismCardPool);
        int nonPrismCharacters = ModelDb.AllCharacters
            .Count(character => character is not PrismCharacter);

        __result = unlockedNonPrismPools == nonPrismCharacters;
        return false;
    }
}

[HarmonyPatch(typeof(Kaleidoscope), nameof(Kaleidoscope.AfterObtained))]
internal static class KaleidoscopeAfterObtainedPatch
{
    [HarmonyPrefix]
    private static bool ExcludePrismCardPool(Kaleidoscope __instance, ref Task __result)
    {
        __result = AfterObtainedWithoutPrism(__instance);
        return false;
    }

    private static async Task AfterObtainedWithoutPrism(Kaleidoscope relic)
    {
        List<Reward> rewards = [];
        CardCreationOptions rerollOptions = CardCreationOptions.ForNonCombatWithDefaultOdds(Array.Empty<CardModel>());

        for (int i = 0; i < relic.DynamicVars.Cards.IntValue; i++)
        {
            List<CardModel> cards = [];
            IEnumerable<CardPoolModel> pools = relic.Owner.UnlockState.CharacterCardPools
                .Where(pool => pool != relic.Owner.Character.CardPool && pool is not PrismCardPool)
                .ToList()
                .StableShuffle(relic.Owner.RunState.Rng.Niche)
                .Take(3);

            foreach (CardPoolModel pool in pools)
            {
                CardCreationOptions options = new CardCreationOptions(
                    [pool],
                    CardCreationSource.Other,
                    CardRarityOddsType.RegularEncounter)
                    .WithFlags(CardCreationFlags.NoCardPoolModifications);

                cards.Add(CardFactory.CreateForReward(relic.Owner, 1, options).First().Card);
            }

            rewards.Add(new CardReward(cards, CardCreationSource.Other, relic.Owner, rerollOptions));
        }

        await RewardsCmd.OfferCustom(relic.Owner, rewards);
    }
}
