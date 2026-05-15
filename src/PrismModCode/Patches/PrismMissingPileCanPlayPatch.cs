using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace PrismMod;

[HarmonyPatch]
internal static class PrismMissingPileCanPlayPatch
{
    private static MethodBase TargetMethod() =>
        AccessTools.Method(
            typeof(CardModel),
            nameof(CardModel.CanPlay),
            [
                typeof(UnplayableReason).MakeByRefType(),
                typeof(AbstractModel).MakeByRefType(),
            ]);

    private static void Postfix(CardModel __instance, ref UnplayableReason reason, ref AbstractModel? preventer)
    {
        if (__instance is PrismCard { IsBlockedByMissingRequiredPile: true } prismCard)
        {
            reason |= UnplayableReason.BlockedByCardLogic;
            preventer = prismCard;
        }
    }
}
