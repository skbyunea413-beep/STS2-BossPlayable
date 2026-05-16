using HarmonyLib;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Modding;

namespace IroncladBossTestMod;

[ModInitializer(nameof(Initialize))]
public class MainFile
{
    public const string ModId = "IroncladBossTestMod";
    public static string ResPath => $"res://{ModId}";
    private static bool _isPatched;

    public static void Initialize()
    {
        if (!_isPatched)
        {
            new Harmony($"{ModId}.harmony").PatchAll(typeof(MainFile).Assembly);
            _isPatched = true;
        }

        _ = new IroncladBossTestCardPool();
        _ = new IroncladBossEvent();
        _ = new IroncladBossEncounter();

        CustomContentDictionary.AddModel(typeof(PhaseSkip));
        CustomContentDictionary.RegisterType(typeof(IroncladBoss));
    }
}
