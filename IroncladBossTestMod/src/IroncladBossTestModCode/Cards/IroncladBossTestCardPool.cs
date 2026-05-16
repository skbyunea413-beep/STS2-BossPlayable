using Godot;

namespace IroncladBossTestMod;

public class IroncladBossTestCardPool : CustomCardPoolModel
{
    public override string Title => "Ironclad Boss Test";
    public override string EnergyColorName => "ironclad";
    public override string CardFrameMaterialPath => "card_frame_red";
    public override Color DeckEntryCardColor => new Color("#d62000");
    public override bool IsColorless => true;
    public override bool IsShared => true;
}
