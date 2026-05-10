using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;

namespace PrismMod;

public sealed class Scar : PrismCard
{
    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/scar.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(3m),
        new CardsVar(3),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Shiv>(),
    ];

    public Scar() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_bloody_impact");
        await CreatureCmd.Damage(
            ctx,
            base.Owner.Creature,
            base.DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            this);
        if (base.CombatState != null)
        {
            await Shiv.CreateInHand(base.Owner, base.DynamicVars.Cards.IntValue, base.CombatState);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
