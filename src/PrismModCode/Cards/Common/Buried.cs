using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Models.Cards;

namespace PrismMod;

public sealed class Buried : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/buried.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<GiantRock>(base.IsUpgraded)];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(13m, ValueProp.Move),
        new DynamicVar("Rocks", 2m),
    ];

    public Buried() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if (base.CombatState == null)
        {
            return;
        }

        List<CardModel> rocks = [];
        for (int i = 0; i < base.DynamicVars["Rocks"].IntValue; i++)
        {
            var rock = base.CombatState.CreateCard<GiantRock>(base.Owner);
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(rock, CardPreviewStyle.None);
            }

            rocks.Add(rock);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(rocks, PileType.Draw, base.Owner, CardPilePosition.Random);
    }

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(3m);
}
