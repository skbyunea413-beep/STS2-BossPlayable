using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;

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
