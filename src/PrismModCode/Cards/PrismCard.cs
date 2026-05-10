namespace PrismMod;

[Pool(typeof(PrismCardPool))]
public abstract class PrismCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : CustomCardModel(cost, type, rarity, target)
{
    private static readonly Dictionary<string, Godot.Texture2D> PortraitCache = [];

    public override CardPoolModel Pool => ModelDb.CardPool<PrismCardPool>();

    public override CardPoolModel VisualCardPool => ModelDb.CardPool<PrismCardPool>();

    public override Godot.Texture2D? CustomPortrait
    {
        get
        {
            string? path = CustomPortraitPath;
            return path == null ? null : LoadPortraitFromPng(path);
        }
    }

    private static Godot.Texture2D? LoadPortraitFromPng(string path)
    {
        if (PortraitCache.TryGetValue(path, out var texture))
        {
            return texture;
        }

        byte[] bytes = Godot.FileAccess.GetFileAsBytes(path);
        if (bytes.Length == 0)
        {
            return null;
        }

        var image = new Godot.Image();
        if (image.LoadPngFromBuffer(bytes) != Godot.Error.Ok)
        {
            return null;
        }

        texture = Godot.ImageTexture.CreateFromImage(image);
        PortraitCache[path] = texture;
        return texture;
    }

    protected Creature? GetAutoPlayTarget(CardModel card)
    {
        var owner = card.Owner;
        var ownerCreature = owner?.Creature;
        var combatState = card.CombatState ?? ownerCreature?.CombatState;
        if (owner == null || ownerCreature == null || combatState == null)
        {
            return null;
        }

        if (card.IsValidTarget(null))
        {
            return null;
        }

        var candidates = card.TargetType switch
        {
            TargetType.AnyEnemy => combatState.HittableEnemies
                .Where(card.IsValidTarget)
                .ToList(),
            TargetType.AnyAlly => combatState.Allies
                .Where(creature => creature.IsAlive && creature.IsPlayer && creature != ownerCreature)
                .Where(card.IsValidTarget)
                .ToList(),
            _ => combatState.Creatures
                .Where(card.IsValidTarget)
                .ToList(),
        };

        if (candidates.Count == 0)
        {
            return null;
        }

        var rng = owner.RunState.Rng.CombatTargets;
        return rng.NextItem(candidates);
    }

    protected bool CanAutoPlayNow(CardModel card) =>
        card.IsValidTarget(null) || GetAutoPlayTarget(card) != null;

    protected Task AutoPlayWithValidTarget(PlayerChoiceContext ctx, CardModel card, bool skipCardPileVisuals = false) =>
        CardCmd.AutoPlay(ctx, card, GetAutoPlayTarget(card), skipCardPileVisuals: skipCardPileVisuals);
}
