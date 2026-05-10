using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;

namespace PrismMod;

[HarmonyPatch(typeof(NTransition), nameof(NTransition.FadeOut))]
internal static class PrismSelectSplashTransitionPatch
{
    [HarmonyPrefix]
    private static void HidePrismSelectSplash(NTransition __instance)
    {
        var tree = __instance.GetTree();
        if (tree == null)
        {
            return;
        }

        foreach (var node in tree.GetNodesInGroup("prism_select_splash"))
        {
            if (node is not CanvasItem splash)
            {
                continue;
            }

            splash.ZIndex = -4096;
            splash.ShowBehindParent = true;
            splash.Visible = false;
        }
    }
}
