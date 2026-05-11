using MegaCrit.Sts2.Core.Entities.Powers;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers
{
    public class BloodSwordArtPower : RebelliaPowers
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override int DisplayAmount => GetInternalData<Data>().BloodPoints;

        protected override object InitInternalData() => new Data();

        public void AddPoints(int amount) => GetInternalData<Data>().BloodPoints += amount;

        public bool TrySpendPoints(int amount)
        {
            var data = GetInternalData<Data>();
            if (data.BloodPoints < amount)
                return false;
            data.BloodPoints -= amount;
            return true;
        }

        public int GetPoints() => GetInternalData<Data>().BloodPoints;

        private class Data
        {
            public int BloodPoints = 0;
        }
    }
}
