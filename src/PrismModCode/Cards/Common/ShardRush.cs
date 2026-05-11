namespace PrismMod;

public sealed class ShardRush : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/shardrush.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new CardsVar(1),
    ];

    public ShardRush() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);

        await PrismRandomCardHelper.AddRandomCardToHand(
            ctx,
            base.Owner,
            card => card.Type == CardType.Attack
                && PrismRandomCardHelper.IsOtherCharacterCard(card)
                && PrismRandomCardHelper.IsPlayableThisTurnAfterShard(base.Owner, card));
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(3m);
}
