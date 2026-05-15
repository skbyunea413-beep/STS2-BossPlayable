using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;

namespace PrismMod;

[HarmonyPatch]
internal static class PrismMissingPileDialoguePatch
{
    private static MethodBase TargetMethod()
    {
        var type = AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.UnplayableReasonExtensions");
        return AccessTools.Method(type, "GetPlayerDialogueLine");
    }

    private static bool Prefix(UnplayableReason reason, AbstractModel? preventer, ref LocString? __result)
    {
        if (preventer is PrismCard { IsBlockedByMissingRequiredPile: true } &&
            reason.HasFlag(UnplayableReason.BlockedByCardLogic))
        {
            __result = new LocString("characters", "PRISM_MOD_CHARACTER_PRISM_CHARACTER.banter.noCardsThere");
            return false;
        }

        return true;
    }
}
