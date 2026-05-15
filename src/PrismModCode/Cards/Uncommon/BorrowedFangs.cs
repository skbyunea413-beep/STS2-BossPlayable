using MegaCrit.Sts2.Core.HoverTips;

namespace PrismMod;

public sealed class BorrowedFangs : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/borrowedfangs.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar("IntentDamage", 5m, ValueProp.Move),
        new RepeatVar(3),
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PrismCardKeywords.AttackIntent),
    ];

    public BorrowedFangs() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var intent = await PowerCmd.Apply<AttackIntentPower>(
            ctx,
            base.Owner.Creature,
            base.DynamicVars["IntentDamage"].BaseValue,
            base.Owner.Creature,
            this);
        if (intent != null)
        {
            intent.DynamicVars.Repeat.BaseValue = base.DynamicVars.Repeat.IntValue;
            intent.SetTargetAllEnemies();
        }

        await WarningColorPower.TriggerForAttackIntent(ctx, base.Owner.Creature, null, true, this);
        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            await PrismRandomCardHelper.AddOtherCharacterCardToHand(ctx, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}
