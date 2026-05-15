namespace PrismMod;

public sealed class ShardFurnace : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/shardfurnace_edit.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2m, ValueProp.Unpowered | ValueProp.Move),
        new RepeatVar(2),
    ];

    public ShardFurnace() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<ShardFurnacePower>(
            ctx,
            base.Owner.Creature,
            1,
            base.Owner.Creature,
            this);
        if (power != null)
        {
            power.Damage = base.DynamicVars.Damage.BaseValue;
            power.Repeat = base.DynamicVars.Repeat.IntValue;
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(1m);
}
