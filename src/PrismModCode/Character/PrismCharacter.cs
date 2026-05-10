using Godot;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Scaffolding.Characters.Visuals.Definition;

namespace PrismMod;

public class PrismCharacter : ModCharacterTemplate<IroncladCardPool, PrismRelicPool, PrismPotionPool>
{
    public const string CharacterId = "PrismMod";
    public static readonly Color CharColor = new Color("#7e57ff");

    public override string PlaceholderCharacterId => "regent";

    public override string? CustomVisualsPath => SceneHelper.GetScenePath("creature_visuals/infested_prism");

    protected override NCreatureVisuals? TryCreateCreatureVisuals()
    {
        var scene = PreloadManager.Cache.GetScene(SceneHelper.GetScenePath("creature_visuals/infested_prism"));
        var visuals = scene.Instantiate<NCreatureVisuals>(PackedScene.GenEditState.Disabled);
        visuals.Ready += () => OnVisualsReady(visuals);
        return visuals;
    }

    private static void OnVisualsReady(NCreatureVisuals visuals)
    {
        var spineBody = visuals.SpineBody;
        if (spineBody?.BoundObject is Node2D spineNode)
        {
            spineNode.Scale = new Vector2(-0.377f, 0.377f);
            spineNode.Position += new Vector2(5f, 90f);
        }
        LogSpineAnimations(visuals);
    }

    private static void LogSpineAnimations(NCreatureVisuals visuals)
    {
        var spineBody = visuals.SpineBody;
        var data = spineBody?.GetSkeleton()?.GetData();
        if (data == null)
        {
            GD.PrintErr("[PrismMod] No spine skeleton data found on InfestedPrism visuals");
            return;
        }
        var names = new System.Collections.Generic.List<string>();
        foreach (var animObjLoop in (System.Collections.Generic.IEnumerable<GodotObject>)data.GetAnimations())
        {
            var animObj = animObjLoop;
            var anim = new MegaAnimation(Variant.From(in animObj));
            var name = anim.GetName();
            if (!string.IsNullOrWhiteSpace(name)) names.Add(name);
        }
        names.Sort(System.StringComparer.OrdinalIgnoreCase);
        GD.Print($"[PrismMod] InfestedPrism spine animations: {string.Join(", ", names)}");
    }

    protected override CreatureAnimator? SetupCustomCreatureAnimator(MegaSprite controller)
    {
        var idle = new AnimState("idle_loop", true);
        var dead = new AnimState("die");
        var hit = new AnimState("hurt") { NextState = idle };
        var attack = new AnimState("attack") { NextState = idle };
        var attackDouble = new AnimState("attack_double") { NextState = idle };
        var cast = new AnimState("buff") { NextState = idle };

        var animator = new CreatureAnimator(idle, controller);
        animator.AddAnyState("Idle", idle);
        animator.AddAnyState("Dead", dead);
        animator.AddAnyState("Hit", hit);
        animator.AddAnyState("Attack", attack);
        animator.AddAnyState("AttackDouble", attackDouble);
        animator.AddAnyState("Cast", cast);
        animator.AddAnyState("Relaxed", idle);
        return animator;
    }


    public override Color NameColor => CharColor;
    public override Color MapDrawingColor => CharColor;
    public override Color EnergyLabelOutlineColor => Colors.Black;
    public override string? CustomIconTexturePath => $"{MainFile.ResPath}/images/charui/character_icon_prism.png";
    public override string? CustomIconOutlineTexturePath => $"{MainFile.ResPath}/images/charui/character_icon_prism_outline.png";
    public override string? CustomIconPath => $"{MainFile.ResPath}/scenes/ui/prism_icon.tscn";
    public override string? CustomMapMarkerPath => $"{MainFile.ResPath}/images/charui/map_marker_prism.png";
    public override string? CustomEnergyCounterPath => $"{MainFile.ResPath}/scenes/ui/prism_energy_counter.tscn";
    public override string? CustomCharacterSelectBgPath => $"{MainFile.ResPath}/scenes/character_select/prism_select_bg.tscn";
    public override string? CustomCharacterSelectIconPath => $"{MainFile.ResPath}/images/charui/char_select_prism.png";
    public override string? CustomCharacterSelectLockedIconPath => $"{MainFile.ResPath}/images/charui/char_select_prism_locked.png";
    public override CharacterWorldProceduralVisualSet? WorldProceduralVisuals =>
        ModCharacterWorldSceneVisuals.Procedural()
            .Merchant(cues => cues
                .Single("idle", $"{MainFile.ResPath}/images/world/shop.png")
                .Single("relaxed_loop", $"{MainFile.ResPath}/images/world/shop.png")
                .Single("die", $"{MainFile.ResPath}/images/world/shop.png"))
            .RestSite(cues => cues
                .Single("overgrowth_loop", $"{MainFile.ResPath}/images/world/rest.png")
                .Single("hive_loop", $"{MainFile.ResPath}/images/world/rest.png")
                .Single("glory_loop", $"{MainFile.ResPath}/images/world/rest.png"))
            .Build();
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 77;
    public override int StartingGold => 77;
    public override bool ShouldAlwaysShowStarCounter => false;
    public override string CharacterSelectSfx =>
        PrismCharacterSelectSfxPatch.SpinSfx;
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";
    public override string? CustomAttackSfx => PrismCharacterSelectSfxPatch.AttackSfx;
    public override string? CustomCastSfx => PrismCharacterSelectSfxPatch.BuffSfx;
    public override string? CustomDeathSfx => PrismCharacterSelectSfxPatch.DeathSfx;
    public override float AttackAnimDelay => 0.15f;
    public override float CastAnimDelay => 0.25f;

    protected override IEnumerable<StartingDeckEntry> StartingDeckEntries =>
    [
        StartingDeckEntry.Of<Reinforce>(6),
        StartingDeckEntry.Of<Guard>(6),
        StartingDeckEntry.Of<PrismWhirlwind>(1),
        StartingDeckEntry.Of<RadiantGamble>(1),
    ];

    protected override IEnumerable<Type> StartingRelicTypes =>
    [
        typeof(PrismaticShard),
        typeof(DingyRug),
        typeof(PrismaticGem),
    ];

    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_starry_impact",
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_lightning",
    ];
}
