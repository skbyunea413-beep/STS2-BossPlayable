using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace PrismMod;

[HarmonyPatch(typeof(NCard), "Reload")]
internal static class PrismCardPortraitUiPatch
{
    private static readonly FieldInfo PortraitField = AccessTools.Field(typeof(NCard), "_portrait");
    private static readonly FieldInfo AncientPortraitField = AccessTools.Field(typeof(NCard), "_ancientPortrait");

    [HarmonyPostfix]
    private static void ForcePrismPortrait(NCard __instance)
    {
        if (__instance.Model is not PrismCard prismCard)
        {
            return;
        }

        Texture2D? portrait = prismCard.CustomPortrait;
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
            if (prismCard.Rarity == CardRarity.Ancient)
            {
                ancientPortraitRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
                ancientPortraitRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;
            }
        }
    }
}
