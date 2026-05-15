namespace PrismMod;

public sealed class GhostNGoblins : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/ghostngoblins.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new SummonVar(2m),
    ];

    public GhostNGoblins() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<GhostNGoblinsPower>(
            ctx,
            base.Owner.Creature,
            1,
            base.Owner.Creature,
            this);
        if (power != null)
        {
            power.Damage = base.DynamicVars.Damage.BaseValue;
            power.Summon = base.DynamicVars.Summon.BaseValue;
        }
    }

    protected override void OnUpgrade() => base.EnergyCost.UpgradeBy(-1);
}
