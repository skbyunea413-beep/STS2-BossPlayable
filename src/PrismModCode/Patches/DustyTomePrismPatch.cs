using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Relics;

namespace PrismMod;

[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.SetupForPlayer))]
internal static class DustyTomeSetupForPrismPatch
{
    [HarmonyPrefix]
    private static bool SetDopamineReward(DustyTome __instance, Player player)
    {
        if (player.Character is not PrismCharacter)
        {
            return true;
        }

        __instance.AncientCard = ModelDb.Card<Dopamine>().Id;
        return false;
    }
}
