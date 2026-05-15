using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Models.Cards;

namespace PrismMod;

public sealed class Scar : PrismCard
{
    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/scar.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Shiv>(base.IsUpgraded),
    ];

    public Scar() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self) { }

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
            var shivsToCreate = System.Math.Max(0, 10 - PileType.Hand.GetPile(base.Owner).Cards.Count);
            List<CardModel> shivs = [];
            for (int i = 0; i < shivsToCreate; i++)
            {
                var shiv = base.CombatState.CreateCard<Shiv>(base.Owner);
                if (base.IsUpgraded)
                {
                    CardCmd.Upgrade(shiv, CardPreviewStyle.None);
                }

                shivs.Add(shiv);
            }

            await CardPileCmd.AddGeneratedCardsToCombat(shivs, PileType.Hand, base.Owner);
        }
    }

    protected override void OnUpgrade() { }
}
