using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace PrismMod;

internal static class PrismAllowedAncientDialogues
{
    private static readonly HashSet<AncientDialogueSet> Sets = [];
    private static readonly HashSet<AncientDialogueSet> OrobasSets = [];
    private static readonly Dictionary<AncientDialogueSet, IReadOnlyList<AncientDialogue>> PrismDialogues = [];
    private static readonly Dictionary<AncientDialogueSet, IReadOnlyList<AncientDialogue>> OrobasPrismDialogues = [];
    private static readonly AncientDialogue SilentDialogue = new("");

    public static void Register(AncientDialogueSet dialogueSet, IReadOnlyList<AncientDialogue>? prismDialogues = null)
    {
        Sets.Add(dialogueSet);
        if (prismDialogues != null)
        {
            PrismDialogues[dialogueSet] = prismDialogues;
        }
    }

    public static bool IsAllowed(AncientDialogueSet dialogueSet)
    {
        return Sets.Contains(dialogueSet);
    }

    public static void RegisterOrobas(AncientDialogueSet dialogueSet, IReadOnlyList<AncientDialogue> prismDialogues)
    {
        Register(dialogueSet, prismDialogues);
        OrobasSets.Add(dialogueSet);
        OrobasPrismDialogues[dialogueSet] = prismDialogues;
    }

    public static void RestorePrismDialogues(AncientDialogueSet dialogueSet)
    {
        if (PrismDialogues.TryGetValue(dialogueSet, out IReadOnlyList<AncientDialogue>? prismDialogues))
        {
            dialogueSet.CharacterDialogues[ModelDb.Character<PrismCharacter>().Id.Entry] = prismDialogues;
        }
    }

    public static bool IsOrobas(AncientDialogueSet dialogueSet)
    {
        return OrobasSets.Contains(dialogueSet);
    }

    public static bool TryGetOrobasPrismDialogues(AncientDialogueSet dialogueSet, out IReadOnlyList<AncientDialogue> prismDialogues)
    {
        return OrobasPrismDialogues.TryGetValue(dialogueSet, out prismDialogues!);
    }

    public static IEnumerable<AncientDialogue> Silent()
    {
        return [SilentDialogue];
    }
}

[HarmonyPatch(typeof(Neow), "DefineDialogues")]
internal static class PrismNeowDialoguePatch
{
    [HarmonyPostfix]
    private static void AddPrismDialogue(AncientDialogueSet __result)
    {
        AncientDialogue[] prismDialogues =
        [
            new AncientDialogue(
                "event:/sfx/npcs/neow/neow_curious",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "event:/sfx/npcs/neow/neow_sleepy")
            {
                VisitIndex = 0,
            },
            new AncientDialogue(
                "event:/sfx/npcs/neow/neow_sleepy",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "event:/sfx/npcs/neow/neow_sleepy")
            {
                VisitIndex = 1,
            },
            new AncientDialogue(
                "event:/sfx/npcs/neow/neow_sleepy",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "event:/sfx/npcs/neow/neow_sleepy")
            {
                VisitIndex = 2,
            },
            new AncientDialogue(
                "event:/sfx/npcs/neow/neow_sleepy",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "event:/sfx/npcs/neow/neow_sleepy")
            {
                VisitIndex = 3,
            },
            new AncientDialogue(
                "event:/sfx/npcs/neow/neow_sleepy",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "event:/sfx/npcs/neow/neow_sleepy")
            {
                VisitIndex = 4,
            },
        ];

        PrismAllowedAncientDialogues.Register(__result, prismDialogues);
        __result.CharacterDialogues[ModelDb.Character<PrismCharacter>().Id.Entry] = prismDialogues;
    }
}

[HarmonyPatch(typeof(AncientDialogueSet), nameof(AncientDialogueSet.PopulateLocKeys))]
internal static class PrismAncientPopulateLocKeysPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    private static void RestorePrismDialoguesAfterBaseLib(AncientDialogueSet __instance)
    {
        PrismAllowedAncientDialogues.RestorePrismDialogues(__instance);
    }
}

[HarmonyPatch(typeof(AncientDialogueSet), nameof(AncientDialogueSet.GetValidDialogues))]
internal static class PrismAncientFirstVisitDialoguePatch
{
    [HarmonyPrefix]
    private static bool PreferPrismDialogueOnFirstVisit(
        AncientDialogueSet __instance,
        ModelId characterId,
        int charVisits,
        bool allowAnyCharacterDialogues,
        ref IEnumerable<AncientDialogue> __result)
    {
        if (characterId.Entry != ModelDb.Character<PrismCharacter>().Id.Entry)
        {
            return true;
        }

        if (!PrismAllowedAncientDialogues.IsAllowed(__instance))
        {
            __result = PrismAllowedAncientDialogues.Silent();
            return false;
        }

        if (PrismAllowedAncientDialogues.IsOrobas(__instance) &&
            charVisits >= 4 &&
            PrismAllowedAncientDialogues.TryGetOrobasPrismDialogues(__instance, out IReadOnlyList<AncientDialogue>? orobasDialogues))
        {
            __result = orobasDialogues
                .Where(dialogue => dialogue.VisitIndex >= 4)
                .ToList();
            return false;
        }

        if (!__instance.CharacterDialogues.TryGetValue(characterId.Entry, out IReadOnlyList<AncientDialogue>? prismDialogues))
        {
            __result = PrismAllowedAncientDialogues.Silent();
            return false;
        }

        List<AncientDialogue> indexedDialogues = prismDialogues
            .Where(dialogue => dialogue.VisitIndex == charVisits)
            .ToList();
        if (indexedDialogues.Count > 0)
        {
            __result = indexedDialogues;
            return false;
        }

        List<AncientDialogue> repeatingDialogues = prismDialogues
            .Where(dialogue => dialogue.IsRepeating && (!dialogue.VisitIndex.HasValue || charVisits >= dialogue.VisitIndex))
            .ToList();
        if (repeatingDialogues.Count > 0)
        {
            __result = repeatingDialogues;
            return false;
        }

        List<AncientDialogue> fallbackDialogues = prismDialogues
            .Where(dialogue => dialogue.VisitIndex.HasValue && dialogue.VisitIndex.Value <= charVisits)
            .OrderByDescending(dialogue => dialogue.VisitIndex!.Value)
            .Take(1)
            .ToList();
        if (fallbackDialogues.Count > 0)
        {
            __result = fallbackDialogues;
            return false;
        }

        __result = PrismAllowedAncientDialogues.Silent();
        return false;
    }
}

[HarmonyPatch(typeof(NAncientEventLayout), nameof(NAncientEventLayout.SetDialogue))]
internal static class PrismAncientDialogueLayoutPatch
{
    [HarmonyPrefix]
    private static bool SkipDisallowedPrismDialogue(NAncientEventLayout __instance, AncientEventModel ____ancientEvent)
    {
        if (____ancientEvent.Owner?.Character is PrismCharacter &&
            !PrismAllowedAncientDialogues.IsAllowed(____ancientEvent.DialogueSet))
        {
            __instance.ClearDialogue();
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(Orobas), "DefineDialogues")]
internal static class PrismOrobasDialoguePatch
{
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void AddPrismDialogue(AncientDialogueSet __result)
    {
        AncientDialogue[] prismDialogues =
        [
            new AncientDialogue(
                "",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 0,
            },
            new AncientDialogue(
                "",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 1,
            },
            new AncientDialogue(
                "",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 2,
            },
            new AncientDialogue(
                "",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 3,
            },
            new AncientDialogue(
                "",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 4,
            },
            new AncientDialogue(
                "",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 5,
            },
            new AncientDialogue(
                "",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 6,
            },
            new AncientDialogue(
                "",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 7,
            },
            new AncientDialogue(
                "",
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 8,
            }
        ];

        PrismAllowedAncientDialogues.RegisterOrobas(__result, prismDialogues);
        __result.CharacterDialogues[ModelDb.Character<PrismCharacter>().Id.Entry] = prismDialogues;
    }
}

[HarmonyPatch(typeof(TheArchitect), "DefineDialogues")]
internal static class PrismArchitectDialoguePatch
{
    [HarmonyPostfix]
    private static void AddPrismDialogue(AncientDialogueSet __result)
    {
        AncientDialogue[] prismDialogues =
        [
            new AncientDialogue(
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 0,
                StartAttackers = ArchitectAttackers.Player,
                EndAttackers = ArchitectAttackers.Architect,
            },
            new AncientDialogue(
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 1,
                StartAttackers = ArchitectAttackers.Player,
                EndAttackers = ArchitectAttackers.Architect,
            },
            new AncientDialogue(
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 2,
                StartAttackers = ArchitectAttackers.Player,
                EndAttackers = ArchitectAttackers.Architect,
            },
            new AncientDialogue(
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 3,
                StartAttackers = ArchitectAttackers.Player,
                EndAttackers = ArchitectAttackers.Architect,
            },
            new AncientDialogue(
                PrismCharacterSelectSfxPatch.SpinSfx,
                "")
            {
                VisitIndex = 4,
                StartAttackers = ArchitectAttackers.Player,
                EndAttackers = ArchitectAttackers.Architect,
            }
        ];

        PrismAllowedAncientDialogues.Register(__result, prismDialogues);
        __result.CharacterDialogues[ModelDb.Character<PrismCharacter>().Id.Entry] = prismDialogues;
    }
}
