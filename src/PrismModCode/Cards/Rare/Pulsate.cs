using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class Pulsate : PrismCard
{
    private const string BuffSfx = "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_buff";

    public Pulsate() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/pulsate.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate, CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PulsatePower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PulsatePower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        SfxCmd.Play(BuffSfx);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", 0.6f);

        await PowerCmd.Apply<PulsatePower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars["PulsatePower"].BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade() => base.DynamicVars["PulsatePower"].UpgradeValueBy(1m);
}
