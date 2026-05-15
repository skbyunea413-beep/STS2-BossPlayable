using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace PrismMod;

[HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.SetupForPlayer))]
internal static class ArchaicToothSetupForPrismPatch
{
    [HarmonyPrefix]
    private static bool AddPrismTranscendence(ArchaicTooth __instance, Player player, ref bool __result)
    {
        if (player.Character is not PrismCharacter)
        {
            return true;
        }

        var starterCard = GetPrismTranscendenceStarter(player);
        if (starterCard == null)
        {
            __result = false;
            return false;
        }

        __instance.SetupForTests(
            starterCard.ToSerializable(),
            CreateAncientCard(starterCard).ToSerializable());
        __result = true;
        return false;
    }

    internal static CardModel? GetPrismTranscendenceStarter(Player player) =>
        (CardModel?)player.Deck.Cards.OfType<RadiantGamble>().FirstOrDefault() ??
        GetPrismWhirlwind(player);

    internal static PrismWhirlwind? GetPrismWhirlwind(Player player) =>
        player.Deck.Cards.OfType<PrismWhirlwind>().FirstOrDefault();

    internal static CardModel CreateAncientCard(CardModel starterCard)
    {
        return starterCard switch
        {
            RadiantGamble radiantGamble => CreateAncientRadiantGamble(radiantGamble),
            PrismWhirlwind whirlwind => CreateAncientWhirlwind(whirlwind),
            _ => starterCard.Owner.RunState.CreateCard<Doubt>(starterCard.Owner)
        };
    }

    internal static AncientRadiantGamble CreateAncientRadiantGamble(RadiantGamble starterCard)
    {
        var ancientCard = starterCard.Owner.RunState.CreateCard<AncientRadiantGamble>(starterCard.Owner);
        CopyStarterCardState(starterCard, ancientCard);
        return ancientCard;
    }

    internal static AncientPrismWhirlwind CreateAncientWhirlwind(PrismWhirlwind starterCard)
    {
        var ancientCard = starterCard.Owner.RunState.CreateCard<AncientPrismWhirlwind>(starterCard.Owner);
        CopyStarterCardState(starterCard, ancientCard);
        return ancientCard;
    }

    private static void CopyStarterCardState(CardModel starterCard, CardModel ancientCard)
    {
        if (starterCard.IsUpgraded)
        {
            CardCmd.Upgrade(ancientCard);
        }

        if (starterCard.Enchantment != null)
        {
            var enchantment = (EnchantmentModel)starterCard.Enchantment.MutableClone();
            CardCmd.Enchant(enchantment, ancientCard, enchantment.Amount);
        }
    }
}

[HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.AfterObtained))]
internal static class ArchaicToothAfterObtainedForPrismPatch
{
    [HarmonyPrefix]
    private static bool TransformPrismStarter(ArchaicTooth __instance, ref Task __result)
    {
        if (__instance.Owner.Character is not PrismCharacter)
        {
            return true;
        }

        __result = TransformPrismStarter(__instance.Owner);
        return false;
    }

    private static async Task TransformPrismStarter(Player player)
    {
        var starterCard = ArchaicToothSetupForPrismPatch.GetPrismTranscendenceStarter(player);
        if (starterCard == null)
        {
            return;
        }

        await CardCmd.Transform(
            starterCard,
            ArchaicToothSetupForPrismPatch.CreateAncientCard(starterCard));
    }
}
