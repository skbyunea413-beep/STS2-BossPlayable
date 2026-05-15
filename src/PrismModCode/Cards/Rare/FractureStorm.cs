namespace PrismMod;

public sealed class FractureStorm : PrismCard
{
    protected override bool ShouldGlowGoldInternal => HasThreshold;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("Threshold", 5m),
    ];

    public FractureStorm() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

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
            .WithHitCount(HasThreshold ? 2 : 1)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(ctx);
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(4m);

    private bool HasThreshold =>
        PileType.Exhaust.GetPile(base.Owner).Cards.Count >= base.DynamicVars["Threshold"].IntValue;
}
