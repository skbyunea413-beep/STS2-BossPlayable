using MegaCrit.Sts2.Core.Combat;

namespace PrismMod;

public sealed class OverchargedLensPower : PrismPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner.Player;
        if (player == null ||
            cardPlay.Card.Owner != player ||
            cardPlay.IsAutoPlay ||
            !PrismRandomCardHelper.HasCostAtLeast(cardPlay.Card, 2))
        {
            return;
        }

        Flash();
        await CardPileCmd.Draw(choiceContext, base.Amount, player);
        await PowerCmd.Remove(this);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (base.Owner.Side == side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
