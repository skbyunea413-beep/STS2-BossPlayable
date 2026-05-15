namespace PrismMod;

public sealed class KaleidoscopeHeart : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/kaleidoscopeheart.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new CardsVar(1),
        new PowerVar<KaleidoscopeHeartPower>(2m),
    ];

    public KaleidoscopeHeart() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<KaleidoscopeHeartPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars["KaleidoscopeHeartPower"].BaseValue,
            base.Owner.Creature,
            this);
        if (power != null)
        {
            power.Block = base.DynamicVars.Block.BaseValue;
            power.Cards = base.DynamicVars.Cards.IntValue;
        }
    }

    protected override void OnUpgrade() => base.DynamicVars["KaleidoscopeHeartPower"].UpgradeValueBy(1m);
}
