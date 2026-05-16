using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace IroncladBossTestMod;

public sealed class IroncladBoss : CustomMonsterModel
{
    private const int PhaseSkipCount = 2;
    private const int PlatingAmount = 12;
    private const int BashDamage = 10;
    private const int BashVulnerable = 3;
    private const int MeltingFistDamage = 14;
    private const int SuppressionVulnerable = 2;
    private const int WhirlwindDamage = 7;
    private const int WhirlwindHits = 3;
    private const string BluntTmpSfx = "blunt_attack.mp3";
    private const string WhirlwindSfx = "event:/sfx/characters/ironclad/ironclad_whirlwind";
    private const string MoltenFistTexturePath = "res://images/vfx/vfx_molten_fist/fist.png";
    private const string MoltenFistBgTexturePath = "res://images/vfx/vfx_molten_fist/fist_bg_v2.png";
    private const string MoltenFistSparkTexturePath = "res://images/vfx/vfx_molten_fist/ground_spark.png";
    private const string MoltenFistRockLightTexturePath = "res://images/vfx/vfx_molten_fist/vfx_rock_light_ver.png";

    public override string? CustomVisualPath => SceneHelper.GetScenePath("creature_visuals/ironclad");

    public override string? CustomAttackSfx => "event:/sfx/characters/ironclad/ironclad_attack";

    public override string? CustomCastSfx => "event:/sfx/card_power";

    protected override string VisualsPath => CustomVisualPath!;

    protected override string AttackSfx => CustomAttackSfx!;

    protected override string CastSfx => CustomCastSfx!;

    public override int MinInitialHp => 300;

    public override int MaxInitialHp => MinInitialHp;

    public override DamageSfxType TakeDamageSfxType => DamageSfxType.ArmorBig;

    public override async Task AfterAddedToRoom()
    {
        FacePlayer();

        await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.25f);
        await PowerCmd.Apply<BarricadePower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);

        await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.25f);
        await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), base.Creature, PlatingAmount, base.Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var setup = new MoveState("PHASE_SKIP_SETUP", PhaseSkipSetupMove, new DebuffIntent());
        var bash = new MoveState("BASH_PLUS", BashPlusMove, new SingleAttackIntent(BashDamage), new DebuffIntent());
        var meltingFist = new MoveState("MELTING_FIST", MeltingFistMove, new SingleAttackIntent(MeltingFistDamage), new DebuffIntent());
        var suppression = new MoveState("SUPPRESSION", SuppressionMove, new DebuffIntent(), new BuffIntent());
        var whirlwind = new MoveState("WHIRLWIND", WhirlwindMove, new MultiAttackIntent(WhirlwindDamage, WhirlwindHits));

        setup.FollowUpState = bash;
        bash.FollowUpState = meltingFist;
        meltingFist.FollowUpState = suppression;
        suppression.FollowUpState = whirlwind;
        whirlwind.FollowUpState = bash;

        return new MonsterMoveStateMachine([setup, bash, meltingFist, suppression, whirlwind], setup);
    }

    private async Task PhaseSkipSetupMove(IReadOnlyList<Creature> targets)
    {
        foreach (Creature target in targets)
        {
            if (target.Player == null)
            {
                continue;
            }

            await CardPileCmd.AddToCombatAndPreview<PhaseSkip>(
                target,
                PileType.Draw,
                PhaseSkipCount,
                target.Player,
                CardPilePosition.Random);
        }
    }

    private async Task BashPlusMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BashDamage).FromMonster(this).WithAttackerAnim("Attack", 0.5f)
            .AfterAttackerAnim(() => PlayFacingTargetsVfx("vfx/vfx_attack_blunt", targets))
            .WithHitFx(null, null, BluntTmpSfx)
            .Execute(null);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, BashVulnerable, base.Creature, null);
    }

    private async Task MeltingFistMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(MeltingFistDamage).FromMonster(this).WithAttackerAnim("Attack", 0.5f)
            .AfterAttackerAnim(() => PlayMeltingFistVfx(targets))
            .WithHitFx("vfx/vfx_attack_blunt", null, BluntTmpSfx)
            .Execute(null);

        foreach (Creature target in targets)
        {
            int vulnerable = target.GetPowerAmount<VulnerablePower>();
            if (vulnerable > 0)
            {
                await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, vulnerable, base.Creature, null);
            }
        }
    }

    private async Task SuppressionMove(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(CastSfx);
        await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.5f);
        VfxCmd.PlayOnCreatureCenter(base.Creature, "vfx/vfx_flying_slash");

        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, SuppressionVulnerable, base.Creature, null);

        int totalVulnerable = targets.Sum(target => target.GetPowerAmount<VulnerablePower>());
        if (totalVulnerable > 0)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, totalVulnerable, base.Creature, null);
        }
    }

    private async Task WhirlwindMove(IReadOnlyList<Creature> targets)
    {
        PlayWhirlwindScreenEffect();
        SfxCmd.Play(WhirlwindSfx);

        await DamageCmd.Attack(WhirlwindDamage).WithHitCount(WhirlwindHits).FromMonster(this)
            .WithAttackerAnim("Attack", 0.35f)
            .AfterAttackerAnim(() => PlayFacingTargetsVfx("vfx/vfx_giant_horizontal_slash", targets))
            .Execute(null);
    }

    private static void PlayWhirlwindScreenEffect()
    {
        Color color = new("FFFFFF80");
        double fastModeDuration = SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast ? 0.8 : 1.1;
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, fastModeDuration));
        NRun.Instance?.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(color, color));
    }

    private Task PlayMeltingFistVfx(IReadOnlyList<Creature> targets)
    {
        foreach (Creature target in targets)
        {
            if (!target.IsAlive)
            {
                continue;
            }

            var targetNode = target.GetCreatureNode();
            if (targetNode == null)
            {
                continue;
            }

            var attackerNode = base.Creature.GetCreatureNode();
            if (attackerNode == null)
            {
                continue;
            }

            Control? container = NCombatRoom.Instance?.CombatVfxContainer ?? target.GetVfxContainer();
            PlayEnemyMoltenFistSprite(container, attackerNode.VfxSpawnPosition, targetNode.VfxSpawnPosition);
            container?.AddChildSafely(NHitSparkVfx.Create(target, requireInteractable: false));
        }

        return Task.CompletedTask;
    }

    private static void PlayEnemyMoltenFistSprite(Control? container, Vector2 start, Vector2 end)
    {
        if (container == null)
        {
            return;
        }

        Texture2D? fistTexture = ResourceLoader.Load<Texture2D>(MoltenFistTexturePath);
        if (fistTexture == null)
        {
            PlayFallbackMoltenFistImpact(container, end);
            return;
        }

        Texture2D? bgTexture = ResourceLoader.Load<Texture2D>(MoltenFistBgTexturePath);
        Texture2D? sparkTexture = ResourceLoader.Load<Texture2D>(MoltenFistSparkTexturePath);
        Texture2D? rockLightTexture = ResourceLoader.Load<Texture2D>(MoltenFistRockLightTexturePath);
        Material moltenMaterial = CreateAdditiveMaterial();

        var root = new Node2D
        {
            GlobalPosition = start + new Vector2(-70f, -24f),
            ZIndex = 50,
        };

        if (bgTexture != null)
        {
            var aura = new Sprite2D
            {
                Texture = bgTexture,
                FlipH = true,
                Scale = new Vector2(1.25f, 1.25f),
                Modulate = new Color(1f, 0.42f, 0.08f, 0.72f),
                Material = moltenMaterial,
            };
            root.AddChild(aura);
        }

        var fist = new Sprite2D
        {
            Texture = fistTexture,
            FlipH = true,
            Scale = new Vector2(0.9f, 0.9f),
            Modulate = new Color(1f, 0.95f, 0.85f, 1f),
        };

        root.AddChild(fist);
        container.AddChildSafely(root);

        SpawnMoltenFistTrail(container, fistTexture, bgTexture, start, end, moltenMaterial);
        SpawnMoltenFistDrips(container, rockLightTexture ?? bgTexture ?? fistTexture, start, end, moltenMaterial);
        SpawnMoltenFistImpact(container, sparkTexture, rockLightTexture, end, moltenMaterial);

        Tween tween = root.CreateTween();
        tween.TweenProperty(root, "global_position", end + new Vector2(18f, -6f), 0.075)
            .SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.InOut);
        tween.Parallel().TweenProperty(root, "scale", new Vector2(1.1f, 1.1f), 0.07)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(fist, "modulate:a", 0.0, 0.06);
        tween.TweenCallback(Callable.From(root.QueueFree));
    }

    private static void PlayFallbackMoltenFistImpact(Control container, Vector2 end)
    {
        Node2D? blunt = CreateVfx("vfx/vfx_attack_blunt");
        if (blunt != null)
        {
            container.AddChildSafely(blunt);
            blunt.GlobalPosition = end;
        }
    }

    private static void SpawnMoltenFistTrail(
        Control container,
        Texture2D fistTexture,
        Texture2D? bgTexture,
        Vector2 start,
        Vector2 end,
        Material moltenMaterial)
    {
        Vector2 origin = start + new Vector2(-70f, -24f);
        Vector2 target = end + new Vector2(18f, -6f);
        for (int i = 0; i < 4; i++)
        {
            float t = i / 4f;
            var afterimage = new Sprite2D
            {
                Texture = i == 0 && bgTexture != null ? bgTexture : fistTexture,
                FlipH = true,
                GlobalPosition = origin.Lerp(target, t) + new Vector2(-38f * (1f - t), 6f * (i % 2 == 0 ? -1f : 1f)),
                Scale = Vector2.One * (1.1f - t * 0.22f),
                Modulate = new Color(1f, 0.25f + t * 0.3f, 0.04f, 0.34f - t * 0.04f),
                Material = moltenMaterial,
                ZIndex = 48,
            };
            container.AddChildSafely(afterimage);

            Tween tween = afterimage.CreateTween();
            tween.TweenProperty(afterimage, "modulate:a", 0.0, 0.12 + t * 0.05);
            tween.Parallel().TweenProperty(afterimage, "scale", afterimage.Scale * 1.45f, 0.12 + t * 0.05);
            tween.TweenCallback(Callable.From(afterimage.QueueFree));
        }
    }

    private static void SpawnMoltenFistDrips(
        Control container,
        Texture2D texture,
        Vector2 start,
        Vector2 end,
        Material moltenMaterial)
    {
        Vector2 origin = start + new Vector2(-64f, 0f);
        Vector2 target = end + new Vector2(16f, 8f);
        for (int i = 0; i < 7; i++)
        {
            float t = i / 6f;
            Vector2 pathPosition = origin.Lerp(target, t);
            var drip = new Sprite2D
            {
                Texture = texture,
                GlobalPosition = pathPosition + new Vector2(-18f + i * 7f, 20f + (i % 3) * 5f),
                Scale = new Vector2(0.18f + i % 2 * 0.05f, 0.38f + i % 3 * 0.08f),
                Rotation = 0.16f - i * 0.05f,
                Modulate = new Color(1f, 0.2f + t * 0.3f, 0.02f, 0.58f - t * 0.16f),
                Material = moltenMaterial,
                ZIndex = 47,
            };
            container.AddChildSafely(drip);

            Tween tween = drip.CreateTween();
            float duration = 0.18f + t * 0.12f;
            tween.TweenProperty(drip, "global_position", drip.GlobalPosition + new Vector2(-18f, 32f + i * 2f), duration)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.In);
            tween.Parallel().TweenProperty(drip, "scale", drip.Scale * new Vector2(0.45f, 1.55f), duration);
            tween.Parallel().TweenProperty(drip, "modulate:a", 0.0, duration);
            tween.TweenCallback(Callable.From(drip.QueueFree));
        }
    }

    private static void SpawnMoltenFistImpact(
        Control container,
        Texture2D? sparkTexture,
        Texture2D? rockLightTexture,
        Vector2 end,
        Material moltenMaterial)
    {
        if (sparkTexture != null)
        {
            var spark = new Sprite2D
            {
                Texture = sparkTexture,
                GlobalPosition = end + new Vector2(8f, 18f),
                Scale = new Vector2(1.2f, 1.2f),
                Modulate = new Color(1f, 0.72f, 0.18f, 0.78f),
                Material = moltenMaterial,
                ZIndex = 52,
            };
            container.AddChildSafely(spark);

            Tween tween = spark.CreateTween();
            tween.TweenProperty(spark, "scale", new Vector2(1.85f, 1.85f), 0.16)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.Out);
            tween.Parallel().TweenProperty(spark, "modulate:a", 0.0, 0.16);
            tween.TweenCallback(Callable.From(spark.QueueFree));
        }

        if (rockLightTexture != null)
        {
            for (int i = 0; i < 3; i++)
            {
                var ember = new Sprite2D
                {
                    Texture = rockLightTexture,
                    GlobalPosition = end + new Vector2(8f, -4f),
                    Rotation = -0.5f + i * 0.5f,
                    Scale = Vector2.One * (0.55f + i * 0.12f),
                    Modulate = new Color(1f, 0.52f, 0.1f, 0.65f),
                    Material = moltenMaterial,
                    ZIndex = 51,
                };
                container.AddChildSafely(ember);

                Tween tween = ember.CreateTween();
                tween.TweenProperty(ember, "global_position", ember.GlobalPosition + new Vector2(24f - i * 18f, -34f - i * 8f), 0.22);
                tween.Parallel().TweenProperty(ember, "modulate:a", 0.0, 0.22);
                tween.TweenCallback(Callable.From(ember.QueueFree));
            }
        }
    }

    private static CanvasItemMaterial CreateAdditiveMaterial()
    {
        return new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.Add,
        };
    }

    private void FacePlayer()
    {
        Node2D? body = base.Creature.GetCreatureNode()?.Visuals.GetCurrentBody();
        if (body == null)
        {
            return;
        }

        body.Scale = new Vector2(-Mathf.Abs(body.Scale.X), body.Scale.Y);
    }

    private Task PlayFacingTargetsVfx(
        string path,
        IReadOnlyList<Creature> targets,
        bool flipX = true,
        Vector2? offsetFromTarget = null)
    {
        foreach (Creature target in targets)
        {
            if (!target.IsAlive)
            {
                continue;
            }

            Node2D? vfx = CreateVfx(path);
            var targetNode = target.GetCreatureNode();
            if (vfx == null || targetNode == null)
            {
                continue;
            }

            Control? container = NCombatRoom.Instance?.CombatVfxContainer ?? target.GetVfxContainer();
            container?.AddChildSafely(vfx);
            vfx.GlobalPosition = targetNode.VfxSpawnPosition + (offsetFromTarget ?? Vector2.Zero);
            vfx.Scale = new Vector2((flipX ? -1f : 1f) * Mathf.Abs(vfx.Scale.X), vfx.Scale.Y);
        }

        return Task.CompletedTask;
    }

    private static Node2D? CreateVfx(string path)
    {
        return PreloadManager.Cache.GetScene(SceneHelper.GetScenePath(path)).Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
    }

}
