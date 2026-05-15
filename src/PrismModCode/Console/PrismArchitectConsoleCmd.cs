using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TextEffects;

namespace PrismMod;

public sealed class PrismArchitectConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "prism_architect";

    public override string Args => "";

    public override string Description => "Jumps directly to The Architect event for PrismMod testing.";

    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer?.RunState == null || !RunManager.Instance.IsInProgress)
        {
            return new CmdResult(success: false, "This command only works during a run.");
        }

        Task task = RunManager.Instance.EnterRoom(new EventRoom(ModelDb.Event<TheArchitect>()));
        return new CmdResult(task, success: true, "Jumped to The Architect.");
    }
}

public sealed class PrismOrobasConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "prism_orobas";

    public override string Args => "";

    public override string Description => "Jumps directly to the Orobas event for PrismMod testing.";

    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer?.RunState == null || !RunManager.Instance.IsInProgress)
        {
            return new CmdResult(success: false, "This command only works during a run.");
        }

        var orobas = ModelDb.Event<Orobas>();
        issuingPlayer.RunState.AppendToMapPointHistory(MapPointType.Ancient, RoomType.Event, orobas.Id);
        Task task = RunManager.Instance.EnterRoom(new EventRoom(orobas));
        return new CmdResult(task, success: true, "Jumped to Orobas.");
    }
}

public sealed class PrismUpgradePreviewDebugConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "prism_upgrade_debug";

    public override string Args => "";

    public override string Description => "Prints Prism upgrade preview text diagnostics.";

    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        var card = ModelDb.Card<Reinforce>().ToMutable();
        card.UpgradeInternal();
        string description = card.GetDescriptionForUpgradePreview();
        string manual = StsTextUtilities.HighlightChangeText(
            card.DynamicVars.Damage.IntValue.ToString(),
            card.DynamicVars.Damage.WasJustUpgraded ? 1 : 0);

        string message =
            $"Reinforce Damage={card.DynamicVars.Damage.IntValue}, " +
            $"WasJustUpgraded={card.DynamicVars.Damage.WasJustUpgraded}, " +
            $"Manual={manual}, Description={description}";

        return new CmdResult(success: true, message);
    }
}
