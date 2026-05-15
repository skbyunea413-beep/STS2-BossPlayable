namespace PrismMod;

public sealed class GhostNGoblinsPower : PrismPower
{
    private bool _dealDamageNext = true;

    internal decimal Damage { get; set; } = 4m;
    internal decimal Summon { get; set; } = 2m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new SummonVar(2m),
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner.Player;
        if (player == null ||
            cardPlay.Card.Owner != player ||
            cardPlay.IsAutoPlay ||
            !PrismRandomCardHelper.IsOtherCharacterCard(cardPlay.Card))
        {
            return;
        }

        Flash();
        if (_dealDamageNext)
        {
            var target = player.RunState.Rng.CombatTargets.NextItem(base.CombatState.HittableEnemies);
            if (target != null)
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    target,
                    Damage,
                    ValueProp.Move,
                    base.Owner,
                    cardPlay.Card);
            }
        }
        else
        {
            await OstyCmd.Summon(choiceContext, player, Summon, this);
        }

        _dealDamageNext = !_dealDamageNext;
    }
}
