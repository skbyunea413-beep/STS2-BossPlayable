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
            .Card<PrismCardPool, AncientRadiantGamble>()
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
            .Card<PrismCardPool, RescueStrike>()
            .Card<PrismCardPool, FacetedGuard>()
            .Card<PrismCardPool, SharpAfterimage>()
            .Card<PrismCardPool, SecretProcurement>()
            .Card<PrismCardPool, ShardFinder>()
            .Card<PrismCardPool, PrismRefraction>()
            .Card<PrismCardPool, ForcedFlare>()
            .Card<PrismCardPool, ShardSalvage>()
            .Card<PrismCardPool, ScatteredStrike>()
            .Card<PrismCardPool, AngularCover>()
            .Card<PrismCardPool, MirrorScreen>()
            .Card<PrismCardPool, BigShard>()
            .Card<PrismCardPool, PrismaticBrand>()
            .Card<PrismCardPool, ChromaticAberration>()
            .Card<PrismCardPool, AfterglowCompression>()
            .Card<PrismCardPool, RefractiveDistortion>()
            .Card<PrismCardPool, OverchargedLens>()
            .Card<PrismCardPool, KaleidoscopeHeart>()
            .Card<PrismCardPool, LightShardShield>()
            .Card<PrismCardPool, LiterallyFemale>()
            .Card<PrismCardPool, BorrowedOrbit>()
            .Card<PrismCardPool, DebrisReaction>()
            .Card<PrismCardPool, DelayedAmplification>()
            .Card<PrismCardPool, HeavyRefraction>()
            .Card<PrismCardPool, InverseFund>()
            .Card<PrismCardPool, PrismRearrangement>()
            .Card<PrismCardPool, PrismaticSpread>()
            .Card<PrismCardPool, RiftStorage>()
            .Card<PrismCardPool, ExternalRefraction>()
            .Card<PrismCardPool, ShardFurnace>()
            .Card<PrismCardPool, LensRunaway>()
            .Card<PrismCardPool, IntentBreaker>()
            .Card<PrismCardPool, PrismGuillotine>()
            .Card<PrismCardPool, FractureStorm>()
            .Card<PrismCardPool, SpectrumLance>()
            .Card<PrismCardPool, PerfectRefraction>()
            .Card<PrismCardPool, Fold>()
            .Card<PrismCardPool, LightBet>()
            .Card<PrismCardPool, BorrowedFootwork>()
            .Card<PrismCardPool, WarningColor>()
            .Card<PrismCardPool, GravityCapacitor>()
            .Card<PrismCardPool, BorrowedEdge>()
            .Relic<PrismRelicPool, PrismaticShard>()
            .Power<AttackIntentPower>()
            .Power<AttackIntentWeakPower>()
            .Power<LiterallyFemaleIntentPower>()
            .Power<PulsatePower>()
            .Power<RadiantGamblePower>()
            .Power<RadiantGambleStrengthPower>()
            .Power<RefractiveDistortionStrengthDownPower>()
            .Power<DopaminePower>()
            .Power<VitalSparkPower>()
            .Power<GentAndFectPower>()
            .Power<GhostNGoblinsPower>()
            .Power<OverchargedLensPower>()
            .Power<FieldProcurementPower>()
            .Power<KaleidoscopeHeartPower>()
            .Power<BorrowedOrbitPower>()
            .Power<ExternalRefractionPower>()
            .Power<ShardFurnacePower>()
            .Power<MirrorScreenPower>()
            .Power<LensRunawayPower>()
            .Power<BorrowedFootworkPower>()
            .Power<WarningColorPower>()
            .Power<GravityCapacitorPower>()
            .Power<InverseFundPower>()
            .Power<TemporaryThornsPower>()
            .Apply();
    }
}
