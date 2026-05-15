namespace PrismMod;

public sealed class ExternalRefractionPower : PrismPower
{
    internal decimal Block { get; set; } = 8m;
    internal int Cards { get; set; } = 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new CardsVar(1),
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner.Player;
        if (player == null ||
            cardPlay.Card.Owner != player ||
            cardPlay.IsAutoPlay ||
            !PrismRandomCardHelper.IsOtherCharacterCard(cardPlay.Card))
        {
            return;
        }

        Flash();
        await CreatureCmd.GainBlock(base.Owner, Block * base.Amount, ValueProp.Move, null);
        await CardPileCmd.Draw(choiceContext, Cards * base.Amount, player);
    }
}
