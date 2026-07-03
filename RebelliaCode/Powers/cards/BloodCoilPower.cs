using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodCoilPower : RebelliaPowers
{
    private int _storedTempHp;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => GetStoredTempHp();
    public override bool ShouldReceiveCombatHooks => true;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override object InitInternalData()
    {
        return new PowerData();
    }

    private PowerData GetData()
    {
        return GetInternalData<PowerData>();
    }

    private void SetStoredTempHp(int value)
    {
        GetData().TempHp = value;
        InvokeDisplayAmountChanged();
    }

    private int GetStoredTempHp()
    {
        var data = GetInternalData<PowerData>();
        return data?.TempHp ?? 0;
    }

    private void SyncTempHpToData()
    {
        var tempPower = Owner?.GetPower<RebelliaTmepHpPower>();
        if (tempPower != null)
        {
            var currentTempHp = tempPower.GetCurrentTempHp();
            if (currentTempHp != GetStoredTempHp())
                SetStoredTempHp(currentTempHp);
        }
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        SyncTempHpToData();
        _storedTempHp = GetStoredTempHp();
        await base.AfterApplied(applier, cardSource);
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side == Owner.Side)
        {
            SyncTempHpToData();
            _storedTempHp = GetStoredTempHp();
        }

        await base.BeforeSideTurnEnd(choiceContext, side, participants);
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
        if (target == Owner)
        {
            SyncTempHpToData();

            var currentTempHp = GetStoredTempHp();
            if (currentTempHp != _storedTempHp && currentTempHp > 0)
            {
                var combatState = Owner.CombatState;
                if (combatState != null)
                {
                    foreach (var enemy in combatState.HittableEnemies)
                        await CreatureCmd.Damage(
                            new BlockingPlayerChoiceContext(),
                            enemy,
                            currentTempHp,
                            ValueProp.Unpowered | ValueProp.SkipHurtAnim,
                            Owner,
                            null,
                            null
                        );
                    _storedTempHp = currentTempHp;
                }
            }
        }

        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
    }

    private class PowerData
    {
        public int TempHp { get; set; }
    }
}
