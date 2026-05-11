using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers
{
    public class BloodSwordArtPower : RebelliaPowers
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;

        public override int DisplayAmount => GetInternalData<Data>().BloodPoints;

        protected override object InitInternalData() => new Data();

        public void AddPoints(int amount)
        {
            var data = GetInternalData<Data>();
            data.BloodPoints += amount;
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

        private class Data
        {
            public int BloodPoints = 0;
        }
    }
}
