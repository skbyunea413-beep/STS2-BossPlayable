using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace PrismMod;

[HarmonyPatch(typeof(NCharacterSelectScreen), nameof(NCharacterSelectScreen.SelectCharacter))]
internal static class PrismCharacterSelectSfxPatch
{
    internal const string SpinSfx = "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_attack_spin";
    internal const string AttackSfx = "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_attack";
    private const string AttackDefendSfx = "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_attack_defend";
    internal const string BuffSfx = "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_buff";
    internal const string DeathSfx = "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_die";

    [HarmonyPostfix]
    private static void LayerPrismSelectSfx(NCharacterSelectButton charSelectButton, CharacterModel characterModel)
    {
        if (charSelectButton.IsRandom || characterModel is not PrismCharacter || NAudioManager.Instance == null)
        {
            return;
        }

        NAudioManager.Instance.PlayOneShot(SpinSfx, 1.65f);
        NAudioManager.Instance.PlayOneShot(AttackSfx, 1.25f);
        NAudioManager.Instance.PlayOneShot(AttackDefendSfx, 1.25f);
        NAudioManager.Instance.PlayOneShot(BuffSfx, 1.35f);
        NAudioManager.Instance.PlayOneShot(DeathSfx, 0.9f);
    }
}
