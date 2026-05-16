using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Runs;

namespace IroncladBossTestMod;

public sealed class IroncladBossTestConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "ironclad_boss_test";

    public override string Args => "";

    public override string Description => "Jumps directly to the standalone Ironclad boss test event.";

    public override bool IsNetworked => true;

    public override bool DebugOnly => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer?.RunState == null || !RunManager.Instance.IsInProgress)
        {
            return new CmdResult(success: false, "This command only works during a run.");
        }

        Task task = RunManager.Instance.EnterRoom(new EventRoom(ModelDb.Event<IroncladBossEvent>()));
        return new CmdResult(task, success: true, "Jumped to Ironclad boss test event.");
    }
}
