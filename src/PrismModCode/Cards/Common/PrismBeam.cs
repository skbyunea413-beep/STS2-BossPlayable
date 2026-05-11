namespace PrismMod;

public sealed class PrismBeam : PrismCard
{
    public override string? CustomPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/prismbeam.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(19m, ValueProp.Move),
    ];

    public PrismBeam() : base(3, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        decimal damage = base.DynamicVars.Damage.BaseValue * DamageMultiplier();

        await DamageCmd.Attack(damage)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .WithAttackerAnim("Attack", base.Owner.Character.AttackAnimDelay, base.Owner.Creature)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(6m);
    }

    private decimal DamageMultiplier()
    {
        int beamCount = CardsWherever().Count(card => card is PrismBeam);
        int otherBeamCount = System.Math.Max(0, beamCount - 1);
        decimal multiplier = 1m;
        for (int i = 0; i < otherBeamCount; i++)
        {
            multiplier *= 2m;
        }

        return multiplier;
    }

    private IEnumerable<CardModel> CardsWherever()
    {
        if (base.Owner.PlayerCombatState != null)
        {
            return base.Owner.PlayerCombatState.AllCards;
        }

        return base.Owner.Deck.Cards;
    }
}
