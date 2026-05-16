using MegaCrit.Sts2.Core.HoverTips;

namespace IroncladBossTestMod;

[Pool(typeof(IroncladBossTestCardPool))]
public sealed class PhaseSkip : CustomCardModel
{
    public override int MaxUpgradeLevel => 0;

    public override bool CanBeGeneratedInCombat => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/phaseskip.png";

    public override Godot.Texture2D? CustomPortrait => Godot.ResourceLoader.Load<Godot.Texture2D>(CustomPortraitPath);

    public override IEnumerable<string> AllPortraitPaths => [CustomPortraitPath!];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    public PhaseSkip() : base(0, CardType.Status, CardRarity.Status, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Remove<VulnerablePower>(base.Owner.Creature);
    }
}
