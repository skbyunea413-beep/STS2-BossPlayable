using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class GentAndFectPower : PrismPower
{
    internal decimal Stars { get; set; } = 2m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StarsVar(2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }

        Flash();
        await PlayerCmd.GainStars(Stars * base.Amount, player);
        for (int i = 0; i < base.Amount; i++)
        {
            await OrbCmd.Channel(
                choiceContext,
                OrbModel.GetRandomOrb(player.RunState.Rng.CombatOrbGeneration).ToMutable(),
                player);
        }
    }
}
