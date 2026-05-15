using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class HeavyRefraction : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/heavyrefraction.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("Energy")
            .WithMultiplier((card, _) => CountCostAtLeastTwoCards(card.Owner)),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [base.EnergyHoverTip];

    public HeavyRefraction() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(ctx);

        await PlayerCmd.GainEnergy(((CalculatedVar)base.DynamicVars["Energy"]).Calculate(cardPlay.Target), base.Owner);
    }

    private static decimal CountCostAtLeastTwoCards(Player player)
    {
        return player.PlayerCombatState.AllCards
            .Count(card => PrismRandomCardHelper.HasCostAtLeast(card, 2));
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(4m);
}
