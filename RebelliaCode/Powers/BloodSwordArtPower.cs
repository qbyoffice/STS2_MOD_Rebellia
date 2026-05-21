using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers;

public class BloodSwordArtPower : RebelliaPowers
{
    public int BloodArtMaxPoints { get; set; } = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => GetInternalData<Data>().BloodArtPoints;
    public override bool ShouldReceiveCombatHooks => true;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void AddPoints(int amount)
    {
        var data = GetInternalData<Data>();
        data.BloodArtPoints = Math.Min(data.BloodArtPoints + amount, BloodArtMaxPoints);
        InvokeDisplayAmountChanged();
    }

    public bool TrySpendPoints(int amount)
    {
        var data = GetInternalData<Data>();
        if (data.BloodArtPoints < amount)
            return false;
        data.BloodArtPoints -= amount;
        InvokeDisplayAmountChanged();
        return true;
    }

    public int GetPoints()
    {
        return GetInternalData<Data>().BloodArtPoints;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await PowerCmd.Remove(this);
    }

    private class Data
    {
        public int BloodArtPoints;
    }
}