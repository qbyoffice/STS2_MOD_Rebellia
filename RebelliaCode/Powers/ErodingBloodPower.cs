using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers;

public class ErodingBloodPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (!participants.Contains(Owner))
            return;
        if (Amount <= 0)
            return;

        var currentHp = Owner.CurrentHp;
        var damage = (int)Math.Ceiling(currentHp * Amount / 100.0);
        if (damage < 1)
            damage = 1;
        if (damage >= currentHp)
            damage = currentHp - 1;

        if (damage > 0)
            await CreatureCmd.Damage(
                new BlockingPlayerChoiceContext(),
                Owner,
                damage,
                ValueProp.Unblockable | ValueProp.Unpowered,
                null,
                null
            );

        SetAmount(Amount - 1);
        InvokeDisplayAmountChanged();
        if (Amount <= 0)
            await PowerCmd.Remove(this);
    }
}
