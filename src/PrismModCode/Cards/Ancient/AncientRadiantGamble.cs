using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace PrismMod;

public sealed class AncientRadiantGamble : PrismCard
{
    private enum FirstEffect
    {
        Damage,
        Block,
        AllEnemiesDamage,
    }

    private enum SecondEffect
    {
        CostReduction,
        Draw,
        RandomOrb,
    }

    private enum ThirdEffect
    {
        Vulnerable,
        Weak,
        TemporaryStrength,
    }

    private FirstEffect _firstEffect;
    private SecondEffect _secondEffect;
    private ThirdEffect _thirdEffect;
    private bool _hasRolled;
    private bool _costReductionApplied;
    private int _costReductionRound = -1;

    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/radiantgamble.png";

    public override IEnumerable<string> AllPortraitPaths => [CustomPortraitPath!];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(24m, ValueProp.Move),
        new BlockVar(15m, ValueProp.Move),
        new CardsVar(3),
        new PowerVar<VulnerablePower>(6m),
        new PowerVar<WeakPower>(3m),
        new PowerVar<StrengthPower>(6m),
        new DamageVar("AllDamage", 18m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    public AncientRadiantGamble() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        RollIfNeeded();
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Attack", base.Owner.Character.AttackAnimDelay);

        switch (_firstEffect)
        {
            case FirstEffect.Damage:
                System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(cardPlay.Target)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(ctx);
                break;
            case FirstEffect.Block:
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
                break;
            case FirstEffect.AllEnemiesDamage:
                if (base.CombatState != null)
                {
                    await DamageCmd.Attack(base.DynamicVars["AllDamage"].BaseValue)
                        .FromCard(this)
                        .TargetingAllOpponents(base.CombatState)
                        .WithHitFx("vfx/vfx_attack_slash")
                        .Execute(ctx);
                }
                break;
        }

        switch (_secondEffect)
        {
            case SecondEffect.CostReduction:
                break;
            case SecondEffect.Draw:
                await CardPileCmd.Draw(ctx, base.DynamicVars.Cards.BaseValue, base.Owner);
                break;
            case SecondEffect.RandomOrb:
                await OrbCmd.Channel(
                    ctx,
                    OrbModel.GetRandomOrb(base.Owner.RunState.Rng.CombatOrbGeneration).ToMutable(),
                    base.Owner);
                break;
        }

        switch (_thirdEffect)
        {
            case ThirdEffect.Vulnerable:
                System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
                await PowerCmd.Apply<VulnerablePower>(
                    ctx,
                    cardPlay.Target,
                    base.DynamicVars.Vulnerable.BaseValue,
                    base.Owner.Creature,
                    this);
                break;
            case ThirdEffect.Weak:
                System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
                await PowerCmd.Apply<WeakPower>(
                    ctx,
                    cardPlay.Target,
                    base.DynamicVars.Weak.BaseValue,
                    base.Owner.Creature,
                    this);
                break;
            case ThirdEffect.TemporaryStrength:
                await PowerCmd.Apply<RadiantGambleStrengthPower>(
                    ctx,
                    base.Owner.Creature,
                    base.DynamicVars.Strength.BaseValue,
                    base.Owner.Creature,
                    this);
                break;
        }
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this)
        {
            return Task.CompletedTask;
        }

        RollEffects();
        var node = NCard.FindOnTable(this, PileType.Hand);
        node?.PlayRandomizeCostAnim();
        node?.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
        return Task.CompletedTask;
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("LineOne", _hasRolled ? "\n[green]" + FormatLine(FirstEffectLocKey()) + "[/green]" : "");
        description.Add("LineTwo", _hasRolled ? "\n[green]" + FormatLine(SecondEffectLocKey()) + "[/green]" : "");
        description.Add("LineThree", _hasRolled ? "\n[green]" + FormatLine(ThirdEffectLocKey()) + "[/green]" : "");
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(9m);
        base.DynamicVars.Block.UpgradeValueBy(9m);
        base.DynamicVars.Cards.UpgradeValueBy(3m);
        base.DynamicVars.Vulnerable.UpgradeValueBy(3m);
        base.DynamicVars.Weak.UpgradeValueBy(3m);
        base.DynamicVars.Strength.UpgradeValueBy(3m);
        base.DynamicVars["AllDamage"].UpgradeValueBy(9m);
    }

    private void RollIfNeeded()
    {
        if (!_hasRolled)
        {
            RollEffects();
        }
    }

    private void RollEffects()
    {
        ClearCurrentRoundCostReduction();

        var rng = base.Owner.RunState.Rng.CombatCardSelection;
        _firstEffect = (FirstEffect)rng.NextInt(3);
        _secondEffect = (SecondEffect)rng.NextInt(3);
        _thirdEffect = (ThirdEffect)rng.NextInt(3);
        _hasRolled = true;

        if (_secondEffect == SecondEffect.CostReduction)
        {
            base.EnergyCost.AddThisTurn(-1, reduceOnly: true);
            _costReductionApplied = true;
            _costReductionRound = base.CombatState?.RoundNumber ?? -1;
        }
    }

    private void ClearCurrentRoundCostReduction()
    {
        int currentRound = base.CombatState?.RoundNumber ?? -1;
        if (_costReductionApplied && _costReductionRound == currentRound)
        {
            base.EnergyCost.AddThisTurn(1);
        }

        _costReductionApplied = false;
        _costReductionRound = -1;
    }

    private string FormatLine(string locKey)
    {
        var line = new LocString("cards", locKey);
        base.DynamicVars.AddTo(line);
        return line.GetFormattedText();
    }

    private string FirstEffectLocKey() =>
        _firstEffect switch
        {
            FirstEffect.Damage => "PRISM_MOD_CARD_RADIANT_GAMBLE.line_damage",
            FirstEffect.Block => "PRISM_MOD_CARD_RADIANT_GAMBLE.line_block",
            _ => "PRISM_MOD_CARD_RADIANT_GAMBLE.line_all_damage",
        };

    private string SecondEffectLocKey() =>
        _secondEffect switch
        {
            SecondEffect.CostReduction => "PRISM_MOD_CARD_RADIANT_GAMBLE.line_cost",
            SecondEffect.Draw => "PRISM_MOD_CARD_RADIANT_GAMBLE.line_draw",
            _ => "PRISM_MOD_CARD_RADIANT_GAMBLE.line_orb",
        };

    private string ThirdEffectLocKey() =>
        _thirdEffect switch
        {
            ThirdEffect.Vulnerable => "PRISM_MOD_CARD_RADIANT_GAMBLE.line_vulnerable",
            ThirdEffect.Weak => "PRISM_MOD_CARD_RADIANT_GAMBLE.line_weak",
            ThirdEffect.TemporaryStrength => "PRISM_MOD_CARD_RADIANT_GAMBLE.line_strength",
            _ => "PRISM_MOD_CARD_RADIANT_GAMBLE.line_strength",
        };
}
