using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib.Content;

namespace PrismMod;

[ModInitializer(nameof(Initialize))]
public class MainFile
{
    public const string ModId = "PrismMod";
    public static string ResPath => $"res://{ModId}";
    private static bool _isPatched;

    public static void Initialize()
    {
        if (!_isPatched)
        {
            new Harmony($"{ModId}.harmony").PatchAll(typeof(MainFile).Assembly);
            _isPatched = true;
        }

        RitsuLibFramework.CreateContentPack(ModId)
            .Epoch<PrismBossEpoch>()
            .Epoch<PrismSecondBossEpoch>()
            .Epoch<PrismFinalBossEpoch>()
            .ModEpochAutoTimelineSlot<PrismBossEpoch>(EpochEra.Invitation5)
            .ModEpochAutoTimelineSlot<PrismSecondBossEpoch>(EpochEra.Invitation5)
            .ModEpochAutoTimelineSlot<PrismFinalBossEpoch>(EpochEra.Invitation5)
            .UnlockEpochAfterBossVictories<PrismCharacter, PrismBossEpoch>(1)
            .Character<PrismCharacter>()
            .SharedCardPool<PrismCardPool>()
            .CardLibraryCompendiumSharedPoolFilter<PrismCardPool>(
                "prism",
                $"{ResPath}/images/charui/character_icon_prism.png",
                [
                    new CardLibraryCompendiumPlacementRule
                    {
                        VanillaFilterAnchorUniqueName = CardLibraryCompendiumVanillaFilterNames.ColorlessPool,
                        Relation = CardLibraryCompendiumFilterInsertRelation.Before,
                    },
                ])
            .Card<PrismCardPool, Reinforce>()
            .Card<PrismCardPool, Guard>()
            .Card<PrismCardPool, PrismWhirlwind>()
            .Card<PrismCardPool, AncientPrismWhirlwind>()
            .Card<PrismCardPool, RadiantGamble>()
            .Card<PrismCardPool, Buried>()
            .Card<PrismCardPool, PrismBeam>()
            .Card<PrismCardPool, FieldProcurement>()
            .Card<PrismCardPool, ShardRush>()
            .Card<PrismCardPool, PrismaticCover>()
            .Card<PrismCardPool, BorrowedFangs>()
            .Card<PrismCardPool, CommonRummage>()
            .Card<PrismCardPool, PeakOfFolly>()
            .Card<PrismCardPool, BorrowedMoment>()
            .Card<PrismCardPool, SparkOfIntent>()
            .Card<PrismCardPool, ExhaustFlare>()
            .Card<PrismCardPool, RecycledGuard>()
            .Card<PrismCardPool, GhostNGoblins>()
            .Card<PrismCardPool, GentAndFect>()
            .Card<PrismCardPool, MixedSignals>()
            .Card<PrismCardPool, ArchmagesRune>()
            .Card<PrismCardPool, Dopamine>()
            .Card<PrismCardPool, HiddenCard>()
            .Card<PrismCardPool, Scar>()
            .Card<PrismCardPool, Radiate>()
            .Card<PrismCardPool, VitalSpark>()
            .Card<PrismCardPool, Pulsate>()
            .Relic<PrismRelicPool, PrismaticShard>()
            .Power<AttackIntentPower>()
            .Power<PulsatePower>()
            .Power<RadiantGamblePower>()
            .Power<DopaminePower>()
            .Power<VitalSparkPower>()
            .Apply();
    }
}
