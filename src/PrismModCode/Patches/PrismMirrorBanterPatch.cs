using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;

namespace PrismMod;

[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom._Ready))]
internal static class PrismMirrorBanterPatch
{
    [HarmonyPostfix]
    private static void ShowMirrorBanter(NCombatRoom __instance)
    {
        if (__instance.Mode != CombatRoomMode.ActiveCombat)
        {
            return;
        }

        var prismPlayers = __instance.CreatureNodes
            .Where(node => node.Entity.Player?.Character is PrismCharacter && !node.Entity.IsDead)
            .ToList();
        var prismEnemies = __instance.CreatureNodes
            .Where(node => IsPrismEnemy(node.Entity.Monster) && !node.Entity.IsDead)
            .ToList();

        if (prismPlayers.Count > 0 && prismEnemies.Count > 0)
        {
            ShowBubble(__instance, prismPlayers[0].Entity, "PRISM_MOD_CHARACTER_PRISM_CHARACTER.banter.mirror.startled");
            ShowBubble(__instance, prismEnemies[0].Entity, "PRISM_MOD_CHARACTER_PRISM_CHARACTER.banter.mirror.incredulous");
            return;
        }

        var prismPlayerPair = prismPlayers
            .Take(2)
            .ToList();
        if (prismPlayerPair.Count < 2)
        {
            return;
        }

        ShowBubble(__instance, prismPlayerPair[0].Entity, "PRISM_MOD_CHARACTER_PRISM_CHARACTER.banter.mirror.startled");
        ShowBubble(__instance, prismPlayerPair[1].Entity, "PRISM_MOD_CHARACTER_PRISM_CHARACTER.banter.mirror.incredulous");
    }

    private static void ShowBubble(NCombatRoom room, MegaCrit.Sts2.Core.Entities.Creatures.Creature speaker, string locKey)
    {
        var text = new LocString("characters", locKey).GetFormattedText();
        var bubble = NSpeechBubbleVfx.Create(text, speaker, 2.25, speaker.Player?.Character.SpeechBubbleColor ?? VfxColor.Cyan);
        if (bubble != null)
        {
            room.CombatVfxContainer.AddChildSafely(bubble);
        }
    }

    private static bool IsPrismEnemy(MegaCrit.Sts2.Core.Models.MonsterModel? monster)
    {
        if (monster == null)
        {
            return false;
        }

        if (monster is InfestedPrism)
        {
            return true;
        }

        var id = monster.Id.Entry;
        if (id.Contains("PRISM", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var title = monster.Title.GetRawText();
        return title.Contains("Prism", StringComparison.OrdinalIgnoreCase) || title.Contains("프리즘");
    }
}
