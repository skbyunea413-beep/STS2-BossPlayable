namespace PrismMod;

[Pool(typeof(PrismRelicPool))]
public abstract class PrismRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{MainFile.ResPath}/images/relics/{GetType().Name.ToLowerInvariant()}.png";

    protected override string PackedIconOutlinePath => $"{MainFile.ResPath}/images/relics/{GetType().Name.ToLowerInvariant()}_outline.png";

    protected override string BigIconPath => $"{MainFile.ResPath}/images/relics/big/{GetType().Name.ToLowerInvariant()}.png";
}
