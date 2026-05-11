namespace PrismMod;

public sealed class BorrowedFangs : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/borrowedfangs.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new RepeatVar(2),
    ];

    public BorrowedFangs() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(base.DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        await PrismRandomCardHelper.AddRandomCardToHand(
            ctx,
            base.Owner,
            card => card.Type == CardType.Attack
                && PrismRandomCardHelper.IsOtherCharacterCard(card)
                && PrismRandomCardHelper.IsPlayableThisTurnAfterShard(base.Owner, card));
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(2m);
}
