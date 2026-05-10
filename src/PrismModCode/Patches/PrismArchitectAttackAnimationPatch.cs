using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models.Events;

namespace PrismMod;

[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.TriggerAnim))]
internal static class PrismArchitectAttackAnimationPatch
{
    [HarmonyPrefix]
    private static void UseDoubleAttackForArchitect(Creature creature, ref string triggerName)
    {
        if (triggerName != "Attack" ||
            !creature.IsPlayer ||
            creature.Player?.Character is not PrismCharacter ||
            creature.Player.RunState.CurrentRoom is not EventRoom eventRoom ||
            eventRoom.CanonicalEvent is not TheArchitect)
        {
            return;
        }

        triggerName = "AttackDouble";
    }
}

[HarmonyPatch(typeof(TheArchitect), "AnimPlayerAttackIfNecessary")]
internal static class PrismArchitectEntryAttackPatch
{
    [HarmonyPrefix]
    private static void ForceEntryAttackForPrism(TheArchitect __instance, ref ArchitectAttackers attackers)
    {
        if (attackers != ArchitectAttackers.None ||
            __instance.Owner?.Character is not PrismCharacter ||
            __instance.Owner.RunState.CurrentRoom is not EventRoom eventRoom ||
            eventRoom.CanonicalEvent is not TheArchitect)
        {
            return;
        }

        int currentLineIndex = Traverse.Create(__instance).Field("_currentLineIndex").GetValue<int>();
        if (currentLineIndex == 0)
        {
            attackers = ArchitectAttackers.Player;
        }
    }
}
