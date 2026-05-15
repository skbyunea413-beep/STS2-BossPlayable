using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace PrismMod;

public sealed class PrismBeam : PrismCard
{
    private const string ChargeSfx = "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_buff";
    private const string FireSfx = "res://PrismMod/audio/sfx/ultra_heavy_laser.ogg";
    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/prismbeam.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
    ];

    public PrismBeam() : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        decimal damage = base.DynamicVars.Damage.BaseValue * DamageMultiplier();
        SfxCmd.Play(ChargeSfx, 0.8f);
        Node2D? beamVfx = PrismMegaBeamVfx.Play(base.Owner.Creature, combatState, ResolveBeamPowerScale());
        PlayWhirlwindScreenEffect();

        await DamageCmd.Attack(damage)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .WithAttackerAnim("Cast", 0.5f, base.Owner.Creature)
            .BeforeDamage(async () =>
            {
                await AnimateBeam(beamVfx, 0f, PrismMegaBeamVfx.ChargeDuration);
                PrismModSfx.Play(FireSfx, 1.1f);
                await AnimateBeam(beamVfx, PrismMegaBeamVfx.ChargeDuration, PrismMegaBeamVfx.ChargeDuration + PrismMegaBeamVfx.DamageImpactDelay);
                NGame.Instance?.ScreenShake(ShakeStrength.TooMuch, ShakeDuration.Short);
            })
            .Execute(ctx);

        await AnimateBeam(beamVfx, PrismMegaBeamVfx.ChargeDuration + PrismMegaBeamVfx.DamageImpactDelay, PrismMegaBeamVfx.ChargeDuration + PrismMegaBeamVfx.BeamDuration);
        PrismMegaBeamVfx.Finish(beamVfx);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }

    private decimal DamageMultiplier()
    {
        int beamCount = CardsWherever().Count(card => card is PrismBeam);
        int otherBeamCount = System.Math.Max(0, beamCount - 1);
        decimal multiplier = 1m;
        for (int i = 0; i < otherBeamCount; i++)
        {
            multiplier *= 2m;
        }

        return multiplier;
    }

    private float ResolveBeamPowerScale()
    {
        int beamCount = CardsWherever().Count(card => card is PrismBeam);
        return System.Math.Min(2.2f, 1f + System.Math.Max(0, beamCount - 1) * 0.22f);
    }

    private static async Task AnimateBeam(Node2D? beamVfx, float from, float to)
    {
        await Cmd.Wait(System.Math.Max(0f, to - from));
    }

    private static void PlayWhirlwindScreenEffect()
    {
        Color color = new("FFFFFF80");
        double duration = PrismMegaBeamVfx.BeamDuration + 0.18;
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, duration));
        NRun.Instance?.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(color, color));
    }

    private IEnumerable<CardModel> CardsWherever()
    {
        if (base.Owner.PlayerCombatState != null)
        {
            return base.Owner.PlayerCombatState.AllCards;
        }

        return base.Owner.Deck.Cards;
    }
}
