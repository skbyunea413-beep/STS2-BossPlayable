namespace PrismMod;

public sealed class InverseFundPower : PrismPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner?.Creature != base.Owner ||
            card.EnergyCost.CostsX ||
            originalCost <= 2m)
        {
            return false;
        }

        modifiedCost = 2m;
        return true;
    }
}
