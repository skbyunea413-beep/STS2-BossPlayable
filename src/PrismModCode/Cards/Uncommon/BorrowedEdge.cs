namespace PrismMod;

public sealed class BorrowedEdge : PrismCard
{
    protected override bool ShouldGlowGoldInternal => CanDraw;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new RepeatVar(2),
        new CardsVar(1),
    ];

    public BorrowedEdge() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(base.DynamicVars.Repeat.IntValue)
            .WithAttackerAnim("AttackDouble", 0.2f, base.Owner.Creature)
            .OnlyPlayAnimOnce()
            .WithAttackerFx(null, PrismWhirlwind.SpinSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);

        if (CanDraw)
        {
            await CardPileCmd.Draw(ctx, base.DynamicVars.Cards.BaseValue, base.Owner);
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(2m);

    private bool CanDraw => PrismCardHistoryHelper.HasPlayedOtherCharacterCardThisTurn(base.Owner);
}
