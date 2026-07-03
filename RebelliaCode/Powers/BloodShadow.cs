using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers;

public class BloodShadow : RebelliaPowers
{
    private bool _eventSubscribed;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => Amount;
    public override bool ShouldReceiveCombatHooks => true;

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DynamicVar("BaseDamage", 10);
            yield return new DynamicVar("TotalDamage", 10 + Amount);
        }
    }

    public static event Func<Player, Task>? SanguineRemoved;

    public static async Task TriggerSanguineRemoved(Player player)
    {
        if (SanguineRemoved != null)
            await SanguineRemoved.Invoke(player);
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (!_eventSubscribed)
        {
            SanguineRemoved += OnSanguineRemoved;
            _eventSubscribed = true;
        }

        DisplayAmountChanged += OnDisplayAmountChanged;
        UpdateTotalDamage();
        await base.AfterApplied(applier, cardSource);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Side)
            return;
        if (combatState.HittableEnemies.Count == 0)
            return;

        var enemies = combatState.HittableEnemies;
        var target = Owner.Player!.RunState.Rng.CombatTargets.NextItem(enemies);
        if (target == null)
            return;

        var totalDamage = (int)DynamicVars["TotalDamage"].BaseValue;
        await CreatureCmd.Damage(
            new BlockingPlayerChoiceContext(),
            target,
            totalDamage,
            ValueProp.Move,
            Owner,
            null,
            null
        );
    }

    private async Task OnSanguineRemoved(Player player)
    {
        if (player != Owner.Player)
            return;
        if (Owner.CombatState == null)
            return;

        var combatState = Owner.CombatState;
        var enemies = combatState.HittableEnemies;
        if (enemies.Count == 0)
            return;

        var target = player.RunState.Rng.CombatTargets.NextItem(enemies);
        if (target == null)
            return;

        var totalDamage = (int)DynamicVars["TotalDamage"].BaseValue;
        await CreatureCmd.Damage(
            new BlockingPlayerChoiceContext(),
            target,
            totalDamage,
            ValueProp.Move,
            Owner,
            null,
            null
        );
    }

    private void OnDisplayAmountChanged()
    {
        UpdateTotalDamage();
    }

    private void UpdateTotalDamage()
    {
        var baseDamage = (int)DynamicVars["BaseDamage"].BaseValue;
        DynamicVars["TotalDamage"].BaseValue = baseDamage + Amount;
    }

    public void SetBaseDamage(int newBase)
    {
        DynamicVars["BaseDamage"].BaseValue = newBase;
        UpdateTotalDamage();
    }

    public int GetTotalDamage()
    {
        return (int)DynamicVars["TotalDamage"].BaseValue;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (_eventSubscribed)
        {
            SanguineRemoved -= OnSanguineRemoved;
            _eventSubscribed = false;
        }

        DisplayAmountChanged -= OnDisplayAmountChanged;
        await base.AfterCombatEnd(room);
    }
}
