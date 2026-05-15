using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class LiterallyFemaleIntentPower : PrismPower
{
    private bool _hasResolved;

    internal int Repeat { get; set; } = 3;

    public override PowerType Type => PowerType.Buff;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new RepeatVar(3),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
    ];

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != base.Owner.Player || _hasResolved)
        {
            return;
        }

        _hasResolved = true;
        Flash();
        await LiterallyFemale.ExecuteRandomHits(
            new BlockingPlayerChoiceContext(),
            base.Owner,
            base.Amount,
            Repeat,
            player.RunState.CreateCard<LiterallyFemale>(player),
            playSpin: true);
        await PowerCmd.Remove(this);
    }
}
