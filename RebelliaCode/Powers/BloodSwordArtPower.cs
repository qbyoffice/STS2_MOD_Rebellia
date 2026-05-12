using MegaCrit.Sts2.Core.Entities.Powers;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers
{
    public class BloodSwordArtPower : RebelliaPowers
    {
        private class Data
        {
            public int BloodPoints = 0;
        }

        public int MaxPoints { get; set; } = 2;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override int DisplayAmount => GetInternalData<Data>().BloodPoints;
        public override bool ShouldReceiveCombatHooks => true;

        protected override object InitInternalData() => new Data();

        public void AddPoints(int amount)
        {
            var data = GetInternalData<Data>();
            data.BloodPoints = System.Math.Min(data.BloodPoints + amount, MaxPoints);
            InvokeDisplayAmountChanged();
        }

        public bool TrySpendPoints(int amount)
        {
            var data = GetInternalData<Data>();
            if (data.BloodPoints < amount)
                return false;
            data.BloodPoints -= amount;
            InvokeDisplayAmountChanged();
            return true;
        }

        public int GetPoints() => GetInternalData<Data>().BloodPoints;
    }
}
