namespace PrismMod;

public sealed class ExhaustFlare : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/exhaustflare.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move)];

    public ExhaustFlare() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        decimal damage = base.DynamicVars.Damage.BaseValue + PileType.Exhaust.GetPile(base.Owner).Cards.Count;
        await DamageCmd.Attack(damage)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(ctx);
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(3m);
}
