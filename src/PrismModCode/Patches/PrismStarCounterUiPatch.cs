using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace PrismMod;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal static class PrismStarCounterUiPatch
{
    private static readonly FieldInfo StarCounterField = AccessTools.Field(typeof(NCombatUi), "_starCounter")!;
    private static readonly FieldInfo EnergyCounterField = AccessTools.Field(typeof(NCombatUi), "_energyCounter")!;
    private static readonly FieldInfo StateField = AccessTools.Field(typeof(NCombatUi), "_state")!;

    private static void Postfix(NCombatUi __instance)
    {
        if (StateField.GetValue(__instance) is not CombatState state)
        {
            return;
        }

        var player = LocalContext.GetMe(state);
        if (player?.Character is not PrismCharacter)
        {
            return;
        }

        if (StarCounterField.GetValue(__instance) is not NStarCounter starCounter ||
            EnergyCounterField.GetValue(__instance) is not NEnergyCounter energyCounter)
        {
            return;
        }

        var starAnchor = energyCounter.GetNodeOrNull<Control>("%StarAnchor");
        if (starAnchor == null)
        {
            return;
        }

        starAnchor.Visible = true;
        starCounter.Reparent(starAnchor, keepGlobalTransform: false);
        starCounter.Position = Vector2.Zero;
    }
}
