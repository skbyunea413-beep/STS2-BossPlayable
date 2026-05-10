using HarmonyLib;
using MegaCrit.Sts2.Core.Saves;

namespace PrismMod;

[HarmonyPatch(typeof(ProgressState), nameof(ProgressState.GetOrCreateCharacterStats))]
internal static class PrismAscensionUnlockPatch
{
    private const int DefaultMaxAscension = 10;

    [HarmonyPostfix]
    private static void UnlockPrismAscension(ModelId characterId, CharacterStats __result)
    {
        if (characterId != ModelDb.Character<PrismCharacter>().Id)
        {
            return;
        }

        if (__result.MaxAscension < DefaultMaxAscension)
        {
            __result.MaxAscension = DefaultMaxAscension;
        }

        if (__result.PreferredAscension < 0 || __result.PreferredAscension > __result.MaxAscension)
        {
            __result.PreferredAscension = __result.MaxAscension;
        }
    }
}
