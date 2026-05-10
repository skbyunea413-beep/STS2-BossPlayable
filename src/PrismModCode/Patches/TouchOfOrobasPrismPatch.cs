using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Relics;

namespace PrismMod;

[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.SetupForPlayer))]
internal static class TouchOfOrobasSetupForPrismPatch
{
    [HarmonyPrefix]
    private static bool AddPrismRefinement(TouchOfOrobas __instance, Player player, ref bool __result)
    {
        if (player.Character is not PrismCharacter)
        {
            return true;
        }

        var starterRelic = player.Relics.OfType<PrismaticShard>().FirstOrDefault();
        if (starterRelic == null)
        {
            return true;
        }

        __instance.SetupForTests(starterRelic.Id, ModelDb.Relic<BurningBlood>().Id);
        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.AfterObtained))]
internal static class TouchOfOrobasAfterObtainedForPrismPatch
{
    [HarmonyPrefix]
    private static bool ObtainBaseStarterRelics(TouchOfOrobas __instance, ref Task __result)
    {
        if (__instance.Owner.Character is not PrismCharacter)
        {
            return true;
        }

        __result = ObtainBaseStarterRelics(__instance.Owner);
        return false;
    }

    private static async Task ObtainBaseStarterRelics(Player player)
    {
        await ObtainIfMissing<BurningBlood>(player);
        await ObtainIfMissing<RingOfTheSnake>(player);
        await ObtainIfMissing<DivineRight>(player);
        await ObtainIfMissing<BoundPhylactery>(player);
        await ObtainIfMissing<CrackedCore>(player);
    }

    private static async Task ObtainIfMissing<T>(Player player)
        where T : RelicModel
    {
        if (player.Relics.Any(relic => relic is T))
        {
            return;
        }

        await RelicCmd.Obtain<T>(player);
    }
}
