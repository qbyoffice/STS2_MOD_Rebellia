using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodBladeTemperPower : RebelliaPowers, IHasSecondAmount
{
    private bool _eventSubscribed;

    protected virtual int BonusPerLost => 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => GetLostLifeCount();
    public override bool ShouldReceiveCombatHooks => true;

    public string GetSecondAmount() => (GetLostLifeCount() * BonusPerLost).ToString();

    private readonly Data _data = new();

    private class Data
    {
        public int LostLifeCount { get; set; }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DynamicVar("LostLifeCount", GetLostLifeCount());
            yield return new DynamicVar("BonusPerLost", BonusPerLost);
            yield return new DynamicVar("TotalBonus", GetLostLifeCount() * BonusPerLost);
        }
    }

    public int GetLostLifeCount() => _data.LostLifeCount;

    public void SetLostLifeCount(int value)
    {
        _data.LostLifeCount = value;
        UpdateDynamicVars();
        InvokeDisplayAmountChanged();
    }

    public void IncrementLostLife()
    {
        _data.LostLifeCount++;
        UpdateDynamicVars();
        InvokeDisplayAmountChanged();
    }

    private void UpdateDynamicVars()
    {
        DynamicVars["LostLifeCount"].BaseValue = GetLostLifeCount();
        DynamicVars["TotalBonus"].BaseValue = GetLostLifeCount() * BonusPerLost;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (!_eventSubscribed)
        {
            RebelliaTmepHpPower.TempHpLost += OnTempHpLost;
            _eventSubscribed = true;
        }
        UpdateDynamicVars();
        await base.AfterApplied(applier, cardSource);
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
        if (target != Owner)
            return;
        if (result.UnblockedDamage > 0)
            IncrementLostLife();
        await Task.CompletedTask;
    }

    private async Task OnTempHpLost(Player player, int lostAmount)
    {
        if (player != Owner.Player)
            return;
        if (lostAmount > 0)
            IncrementLostLife();
        await Task.CompletedTask;
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (dealer != Owner)
            return 0m;
        if (cardSource == null)
            return 0m;
        if (!cardSource.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon))
            return 0m;
        return GetLostLifeCount() * BonusPerLost;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (_eventSubscribed)
        {
            RebelliaTmepHpPower.TempHpLost -= OnTempHpLost;
            _eventSubscribed = false;
        }
        await PowerCmd.Remove(this);
    }
}
