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
    internal const float ChargeDuration = 0.34f;
    internal const float BeamDuration = 1.17f;
    internal const float DamageImpactDelay = 0.86f;
    private const float InGameEffectTimeScale = 0.9f;
    private const string ScenePath = "res://PrismMod/scenes/vfx/prism_mega_beam.tscn";
    private static readonly Vector2 ForwardBeamOffset = new(1600f, 0f);

    public static Node2D? Play(Creature attacker, ICombatState combatState, float powerScale)
    {
        if (TestMode.IsOn)
        {
            return null;
        }

        NCreature? attackerNode = attacker.GetCreatureNode();
        Control? container = NCombatRoom.Instance?.CombatVfxContainer ?? attacker.GetVfxContainer();
        if (attackerNode == null || container == null)
        {
            return null;
        }

        Vector2 from = attackerNode.VfxSpawnPosition + new Vector2(84f, -8f);
        Vector2 to = from + ForwardBeamOffset;

        PackedScene? scene = ResourceLoader.Load<PackedScene>(ScenePath);
        if (scene == null)
        {
            GD.PushWarning($"Failed to load Prism mega beam scene: {ScenePath}");
            return null;
        }

        Node2D beam = scene.Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        beam.ProcessMode = Node.ProcessModeEnum.Always;
        beam.ZIndex = 900;
        beam.Call("setup", Vector2.Zero, to - from, Mathf.Clamp(powerScale, 0.85f, 2.1f));
        beam.Call("set_effect_time_scale", InGameEffectTimeScale);
        container.AddChildSafely(beam);
        beam.GlobalPosition = from;

        attackerNode.AnimShake();
        foreach (Creature target in combatState.GetCreaturesOnSide(CombatSide.Enemy).Where(creature => creature.IsHittable))
        {
            target.GetCreatureNode()?.AnimShake();
        }

        return beam;
    }

    public static void Finish(Node2D? beam)
    {
        if (beam == null || !GodotObject.IsInstanceValid(beam))
        {
            return;
        }

        beam.Call("finish");
        beam.QueueFreeSafely();
    }
}
