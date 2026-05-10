using Godot;

namespace PrismMod;

public class PrismCardPool : TypeListCardPoolModel
{
    public override string Title => PrismCharacter.CharacterId;
    public override string EnergyColorName => "prism";
    public override string? BigEnergyIconPath => $"{MainFile.ResPath}/images/charui/big_energy.png";
    public override string? TextEnergyIconPath => $"{MainFile.ResPath}/images/charui/text_energy.png";
    public override string CardFrameMaterialPath => "card_frame_red";
    public override Color DeckEntryCardColor => new Color("#d62000");
    public override bool IsColorless => false;
}
