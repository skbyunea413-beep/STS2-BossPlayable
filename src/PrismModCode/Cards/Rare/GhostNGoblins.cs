namespace PrismMod;

public sealed class GhostNGoblins : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/ghostngoblins.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SummonVar(3m),
        new ForgeVar(3),
    ];

    public GhostNGoblins() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        int repeats = PileType.Exhaust.GetPile(base.Owner).Cards.Count;
        for (int i = 0; i < repeats; i++)
        {
            await OstyCmd.Summon(ctx, base.Owner, base.DynamicVars.Summon.BaseValue, this);
            await ForgeCmd.Forge(base.DynamicVars.Forge.IntValue, base.Owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Summon.UpgradeValueBy(1m);
        base.DynamicVars.Forge.UpgradeValueBy(1m);
    }
}
