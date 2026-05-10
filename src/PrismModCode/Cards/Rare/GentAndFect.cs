namespace PrismMod;

public sealed class GentAndFect : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/gentandfect.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StarsVar(7),
        new PowerVar<FocusPower>(5m),
        new DynamicVar("OrbSlots", 3m),
    ];

    public GentAndFect() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PlayerCmd.GainStars(base.DynamicVars.Stars.BaseValue, base.Owner);
        await PowerCmd.Apply<FocusPower>(ctx, base.Owner.Creature, base.DynamicVars["FocusPower"].BaseValue, base.Owner.Creature, this);
        await OrbCmd.AddSlots(base.Owner, base.DynamicVars["OrbSlots"].IntValue);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Stars.UpgradeValueBy(2m);
        base.DynamicVars["FocusPower"].UpgradeValueBy(2m);
        base.DynamicVars["OrbSlots"].UpgradeValueBy(2m);
    }
}
