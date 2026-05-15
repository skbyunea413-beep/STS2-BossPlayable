namespace PrismMod;

public sealed class BorrowedOrbit : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/borrowedorbit.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public BorrowedOrbit() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BorrowedOrbitPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars.Cards.BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}
