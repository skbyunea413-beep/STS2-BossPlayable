namespace PrismMod;

public sealed class RefractiveDistortionStrengthDownPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<RefractiveDistortion>();

    protected override bool IsPositive => false;
}
