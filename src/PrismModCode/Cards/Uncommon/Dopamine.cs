namespace PrismMod;

public sealed class Dopamine : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/dopamine.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new EnergyVar("FreeCost", 0),
        new PowerVar<DopaminePower>(2m),
    ];

    public Dopamine() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self) { }

    protected override void OnUpgrade() => base.EnergyCost.UpgradeBy(-1);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = await PrismRandomCardHelper.AddRandomCardToHand(ctx, base.Owner);
        card?.SetToFreeThisTurn();
        await PowerCmd.Apply<DopaminePower>(ctx, base.Owner.Creature, base.DynamicVars["DopaminePower"].BaseValue, base.Owner.Creature, this);
    }
}
