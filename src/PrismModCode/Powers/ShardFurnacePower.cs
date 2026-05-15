using MegaCrit.Sts2.Core.Combat;

namespace PrismMod;

public sealed class ShardFurnacePower : PrismPower
{
    internal decimal Damage { get; set; } = 2m;
    internal int Repeat { get; set; } = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2m, ValueProp.Unpowered | ValueProp.Move),
        new RepeatVar(2),
    ];

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        var player = base.Owner.Player;
        if (base.Owner.Side != side ||
            base.Owner.IsDead ||
            player == null ||
            base.CombatState == null)
        {
            return;
        }

        var cards = PileType.Hand.GetPile(player).Cards.ToList();
        if (cards.Count == 0)
        {
            return;
        }

        Flash();
        foreach (var card in cards)
        {
            await CardCmd.Exhaust(choiceContext, card);
            await DamageCmd.Attack(Damage * base.Amount)
                .FromCard(player.RunState.CreateCard<ShardFurnace>(player))
                .TargetingAllOpponents(base.CombatState)
                .WithHitCount(Repeat)
                .Unpowered()
                .WithHitFx("vfx/vfx_starry_impact")
                .Execute(choiceContext);
        }
    }
}
