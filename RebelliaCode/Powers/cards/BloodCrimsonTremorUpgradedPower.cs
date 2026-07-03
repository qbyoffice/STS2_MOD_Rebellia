using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

internal class BloodCrimsonTremorUpgradedPower : RebelliaPowers
{
    private const int BaseDamage = 8;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => BaseDamage * Amount;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        RebelliaTmepHpPower.TempHpGained += OnTempHpGained;
        await Task.CompletedTask;
    }

    private async Task OnTempHpGained(Player player, int gainedAmount, CardModel? cardSource)
    {
        if (player != Owner.Player)
            return;
        if (gainedAmount <= 0)
            return;

        var combatState = Owner.CombatState;
        if (combatState == null)
            return;

        var totalDamage = BaseDamage * Amount;
        var enemies = combatState.HittableEnemies;
        if (enemies.Count == 0)
            return;

        foreach (var enemy in enemies)
            await CreatureCmd.Damage(
                new BlockingPlayerChoiceContext(),
                enemy,
                totalDamage,
                ValueProp.Move,
                Owner,
                null,
                null
            );
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        RebelliaTmepHpPower.TempHpGained += OnTempHpGained;
        await PowerCmd.Remove(this);
    }
}
