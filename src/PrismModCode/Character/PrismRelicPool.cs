namespace PrismMod;

public class PrismRelicPool : TypeListRelicPoolModel
{
    public override string EnergyColorName => "prism";
    public override string? BigEnergyIconPath => $"{MainFile.ResPath}/images/charui/big_energy.png";
    public override string? TextEnergyIconPath => $"{MainFile.ResPath}/images/charui/text_energy.png";
}
