using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class GentAndFect : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/gentandfect.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StarsVar(2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
    ];

    public GentAndFect() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<GentAndFectPower>(
            ctx,
            base.Owner.Creature,
            1,
            base.Owner.Creature,
            this);
        if (power != null)
        {
            power.Stars = base.DynamicVars.Stars.BaseValue;
        }
    }

    protected override void OnUpgrade() => base.EnergyCost.UpgradeBy(-1);
}
