using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace PrismMod;

public sealed class PulsatePower : PrismPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        var player = base.Owner.Player;
        if (base.Owner.IsDead || player is null)
        {
            return;
        }

        Flash();
        for (int i = 0; i < base.Amount; i++)
        {
            await PrismRandomCardHelper.AutoPlayRandomCard(choiceContext, player);
        }
    }
}
