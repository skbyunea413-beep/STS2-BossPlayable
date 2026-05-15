using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace PrismMod;

public sealed class RefractiveDistortion : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/refractivedistortion.png";

    protected override bool ShouldGlowGoldInternal => CanReduceStrength;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Power", 2m),
        new PowerVar<StrengthPower>(2m),
    ];

    public RefractiveDistortion() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<VulnerablePower>(
            ctx,
            cardPlay.Target,
            base.DynamicVars["Power"].BaseValue,
            base.Owner.Creature,
            this);

        if (CanReduceStrength)
        {
            await PowerCmd.Apply<RefractiveDistortionStrengthDownPower>(
                ctx,
                cardPlay.Target,
                base.DynamicVars.Strength.BaseValue,
                base.Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Strength.UpgradeValueBy(1m);

    private bool CanReduceStrength => PrismCardHistoryHelper.HasGeneratedCardThisTurn(base.Owner);
}
