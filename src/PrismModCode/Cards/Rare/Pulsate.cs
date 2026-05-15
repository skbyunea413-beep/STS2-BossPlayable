using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class Pulsate : PrismCard
{
    private const string BuffSfx = "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_buff";

    public Pulsate() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/pulsate_edit.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [PrismCardKeywords.BattleStart];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12m, ValueProp.Move),
        new PowerVar<StrengthPower>(4m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PulsatePower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ApplyPulsatePower(ctx, fromBattleStart: false);
    }

    private async Task ApplyPulsatePower(PlayerChoiceContext ctx, bool fromBattleStart)
    {
        if (!fromBattleStart)
        {
            SfxCmd.Play(BuffSfx);
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", 0.6f);
        }

        var power = await PowerCmd.Apply<PulsatePower>(
            ctx,
            base.Owner.Creature,
            1,
            base.Owner.Creature,
            fromBattleStart ? null : this,
            silent: fromBattleStart);
        if (power != null)
        {
            power.Block = base.DynamicVars.Block.BaseValue;
            power.Strength = base.DynamicVars["StrengthPower"].BaseValue;
        }
    }

    public override Task BeforeCombatStart()
    {
        if (base.CombatState == null || base.Pile?.Type != PileType.Draw)
        {
            return Task.CompletedTask;
        }

        return ApplyPulsatePower(new ThrowingPlayerChoiceContext(), fromBattleStart: true);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
        base.DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }
}
