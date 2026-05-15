using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class SharpAfterimage : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/sharpafterimage.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new DamageVar(8m, ValueProp.Move),
    ];

    public SharpAfterimage() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, base.DynamicVars.Cards.BaseValue, base.Owner);

        var intent = await PowerCmd.Apply<AttackIntentPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars.Damage.BaseValue,
            base.Owner.Creature,
            this);
        if (cardPlay.Target != null)
        {
            intent?.SetTarget(cardPlay.Target);
            await WarningColorPower.TriggerForAttackIntent(ctx, base.Owner.Creature, cardPlay.Target, false, this);
        }
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(4m);
}
