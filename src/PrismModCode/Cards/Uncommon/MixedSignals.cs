namespace PrismMod;

public sealed class MixedSignals : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/mixedsignals.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new BlockVar(5m, ValueProp.Move),
        new CardsVar(2),
    ];

    public MixedSignals() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            var generated = await PrismRandomCardHelper.AddOtherCharacterCardToHand(
                ctx,
                base.Owner,
                card => card.Type is CardType.Attack or CardType.Skill);
            if (generated == null)
            {
                return;
            }

            if (generated.Type == CardType.Attack)
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
                continue;
            }

            if (generated.Type == CardType.Skill)
            {
                var target = base.CombatState?.HittableEnemies.Count > 0
                    ? base.Owner.RunState.Rng.CombatTargets.NextItem(base.CombatState.HittableEnemies)
                    : null;
                if (target != null)
                {
                    await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                        .FromCard(this)
                        .Targeting(target)
                        .WithHitFx("vfx/vfx_attack_slash")
                        .Execute(ctx);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars.Block.UpgradeValueBy(2m);
    }
}
