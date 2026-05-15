namespace PrismMod;

public sealed class OverchargedLens : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/overchargedlens.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Draw", 1m),
        new CardsVar(2),
    ];

    public OverchargedLens() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await CardPileCmd.Draw(ctx, base.DynamicVars["Draw"].BaseValue, base.Owner);
        await PowerCmd.Apply<OverchargedLensPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars.Cards.BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade() => base.DynamicVars.Cards.UpgradeValueBy(1m);
}
