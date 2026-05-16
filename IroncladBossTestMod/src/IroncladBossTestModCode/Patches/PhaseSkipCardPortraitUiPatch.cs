using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace IroncladBossTestMod;

[HarmonyPatch(typeof(NCard), "Reload")]
internal static class PhaseSkipCardPortraitUiPatch
{
    private static readonly FieldInfo PortraitField = AccessTools.Field(typeof(NCard), "_portrait");
    private static readonly FieldInfo AncientPortraitField = AccessTools.Field(typeof(NCard), "_ancientPortrait");

    [HarmonyPostfix]
    private static void ForcePhaseSkipPortrait(NCard __instance)
    {
        if (__instance.Model is not PhaseSkip phaseSkip)
        {
            return;
        }

        Texture2D? portrait = phaseSkip.CustomPortrait;
        if (portrait == null)
        {
            return;
        }

        if (PortraitField.GetValue(__instance) is TextureRect portraitRect)
        {
            portraitRect.Texture = portrait;
        }

        if (AncientPortraitField.GetValue(__instance) is TextureRect ancientPortraitRect)
        {
            ancientPortraitRect.Texture = portrait;
        }
    }
}
