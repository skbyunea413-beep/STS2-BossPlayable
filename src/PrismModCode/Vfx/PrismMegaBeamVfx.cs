using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace PrismMod;

internal static class PrismMegaBeamVfx
{
    private const string ScenePath = "res://PrismMod/scenes/vfx/prism_mega_beam.tscn";

    public static void Play(Creature attacker, ICombatState combatState, float powerScale)
    {
        if (TestMode.IsOn)
        {
            return;
        }

        NCreature? attackerNode = attacker.GetCreatureNode();
        Control? container = NCombatRoom.Instance?.CombatVfxContainer ?? attacker.GetVfxContainer();
        if (attackerNode == null || container == null)
        {
            return;
        }

        Vector2 from = attackerNode.VfxSpawnPosition + new Vector2(84f, -8f);
        Vector2 to = VfxCmd.GetSideCenter(CombatSide.Enemy, combatState) ?? from + new Vector2(1180f, -40f);
        to += new Vector2(-20f, -8f);

        PackedScene? scene = ResourceLoader.Load<PackedScene>(ScenePath);
        Node2D? beam = scene?.Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        if (beam == null)
        {
            GD.PrintErr($"[PrismMod] Could not load Prism mega beam scene: {ScenePath}");
            return;
        }

        container.AddChildSafely(beam);
        beam.GlobalPosition = Vector2.Zero;
        beam.Call("setup", from, to, powerScale);

        attackerNode.AnimShake();
        foreach (Creature target in combatState.GetCreaturesOnSide(CombatSide.Enemy).Where(creature => creature.IsHittable))
        {
            target.GetCreatureNode()?.AnimShake();
        }
    }
}
