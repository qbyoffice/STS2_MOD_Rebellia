using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class CrimsonHuskPower : RebelliaPowers
{
    private int _DamageThisTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => _DamageThisTurn;
    public override bool ShouldReceiveCombatHooks => true;

    protected override object InitInternalData()
    {
        return new Data();
    }

    private Data GetData()
    {
        return GetInternalData<Data>();
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (target == Owner && result.UnblockedDamage > 0)
        {
            _DamageThisTurn += result.UnblockedDamage;
            GetData().Damage += result.UnblockedDamage;
            InvokeDisplayAmountChanged();
        }

        await Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side == Owner.Side && participants.Contains(Owner))
        {
            var damage = _DamageThisTurn;
            if (damage > 0)
            {
                var tempHpPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner);
                tempHpPower?.AddTempHp(damage);
            }

            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _DamageThisTurn = 0;
        GetData().Damage = 0;
        await Task.CompletedTask;
    }

    private class Data
    {
        public int Damage;
    }
}