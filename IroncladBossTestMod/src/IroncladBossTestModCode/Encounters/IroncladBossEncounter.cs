using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Acts;

namespace IroncladBossTestMod;

public sealed class IroncladBossEncounter : CustomEncounterModel
{
    public IroncladBossEncounter() : base(RoomType.Monster) { }

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<IroncladBoss>()];

    public override bool IsValidForAct(ActModel act) => act is Glory;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        return [(ModelDb.Monster<IroncladBoss>().ToMutable(), null)];
    }
}
