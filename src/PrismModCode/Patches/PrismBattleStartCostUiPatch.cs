using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace PrismMod;

[HarmonyPatch(typeof(NCard), "UpdateEnergyCostVisuals")]
internal static class PrismBattleStartCostUiPatch
{
    private static readonly FieldInfo EnergyLabelField = AccessTools.Field(typeof(NCard), "_energyLabel");
    private static readonly FieldInfo EnergyIconField = AccessTools.Field(typeof(NCard), "_energyIcon");
    private static readonly FieldInfo UnplayableEnergyIconField = AccessTools.Field(typeof(NCard), "_unplayableEnergyIcon");

    [HarmonyPostfix]
    private static void HideBattleStartCost(NCard __instance)
    {
        if (!__instance.Model.Keywords.Contains(PrismCardKeywords.BattleStart))
        {
            if (EnergyLabelField.GetValue(__instance) is CanvasItem visibleEnergyLabel)
            {
                visibleEnergyLabel.Visible = true;
            }

            return;
        }

        if (EnergyLabelField.GetValue(__instance) is CanvasItem energyLabel)
        {
            energyLabel.Visible = false;
        }

        if (EnergyIconField.GetValue(__instance) is CanvasItem energyIcon)
        {
            energyIcon.Visible = false;
        }

        if (UnplayableEnergyIconField.GetValue(__instance) is CanvasItem unplayableEnergyIcon)
        {
            unplayableEnergyIcon.Visible = false;
        }
    }
}
