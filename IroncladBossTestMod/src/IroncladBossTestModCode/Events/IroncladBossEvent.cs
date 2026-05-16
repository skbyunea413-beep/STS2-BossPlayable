using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Rewards;

namespace IroncladBossTestMod;

public sealed class IroncladBossEvent : CustomEventModel
{
    private const string EventKey = "IRONCLADBOSSTESTMOD-IRONCLAD_BOSS_EVENT";
    private const string FightOptionKey = EventKey + ".pages.INITIAL.options.FIGHT";
    private const string RunOptionKey = EventKey + ".pages.INITIAL.options.RUN";

    public override string? CustomInitialPortraitPath => $"{MainFile.ResPath}/images/events/BandiView_gasgasasgasg.png";

    public override bool IsShared => true;

    public override EncounterModel CanonicalEncounter => ModelDb.Encounter<IroncladBossEncounter>();

    public override LocString InitialDescription
    {
        get
        {
            LocString description = L10NLookup(base.Id.Entry + ".pages.INITIAL.description");
            if (base.Owner != null)
            {
                description.Add("TargetName", base.Owner.Character.Title);
            }
            else
            {
                description.Add("TargetName", "Prism");
            }

            return description;
        }
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            CreateOption(Fight, FightOptionKey),
            CreateOption(Run, RunOptionKey),
        ];
    }

    public override Task Resume(AbstractRoom room)
    {
        SetEventFinished(L10NLookup(EventKey + ".pages.VICTORY.description"));
        return Task.CompletedTask;
    }

    private Task Fight()
    {
        IReadOnlyList<Reward> rewards = base.Owner == null ? Array.Empty<Reward>() : [new RelicReward(base.Owner)];
        EnterCombatWithoutExitingEvent<IroncladBossEncounter>(rewards, shouldResumeAfterCombat: true);
        return Task.CompletedTask;
    }

    private EventOption CreateOption(Func<Task> onChosen, string textKey)
    {
        return new EventOption(
            this,
            onChosen,
            new LocString(LocTable, textKey + ".title"),
            new LocString(LocTable, textKey + ".description"),
            textKey,
            Array.Empty<IHoverTip>());
    }

    private async Task Run()
    {
        if (base.Owner != null)
        {
            VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_attack_blunt");
            NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Short);

            await CreatureCmd.Damage(
                new ThrowingPlayerChoiceContext(),
                base.Owner.Creature,
                32m,
                ValueProp.Unblockable | ValueProp.Unpowered,
                null,
                null);
        }

        SetEventFinished(L10NLookup(EventKey + ".pages.RUN.description"));
    }
}
