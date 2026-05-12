using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers
{
    public class CrimsonVeilPower : RebelliaPowers
    {
        private class Data
        {
            public int VeilPoints = 0;
        }

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override int DisplayAmount => GetInternalData<Data>().VeilPoints;
        public override bool ShouldReceiveCombatHooks => true;

        protected override object InitInternalData() => new Data();

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            new[] { new PowerVar<CrimsonVeilPower>(1) };

        public void AddVeilPoints(int amount)
        {
            var data = GetInternalData<Data>();
            data.VeilPoints = System.Math.Max(0, data.VeilPoints + amount);
            InvokeDisplayAmountChanged();
        }

        public int GetVeilPoints() => GetInternalData<Data>().VeilPoints;

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (Owner.Side != side)
                return;

            int veilCount = GetVeilPoints();
            if (veilCount <= 0)
                return;

            decimal conversionRate = DynamicVarsHelper
                .GetPowerVar<CrimsonVeilPower>(DynamicVars)
                .BaseValue;
            int totalToAdd = (int)(veilCount * conversionRate);
            if (totalToAdd > 0)
            {
                var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner);
                if (bloodPower != null)
                {
                    int currentBlood = bloodPower.GetPoints();
                    int maxBlood = bloodPower.MaxPoints;
                    int toAdd = System.Math.Min(totalToAdd, maxBlood - currentBlood);
                    if (toAdd > 0)
                        bloodPower.AddPoints(toAdd);
                }
            }

            AddVeilPoints(-1);
        }

        public override async Task AfterCombatEnd(CombatRoom room)
        {
            await PowerCmd.Remove(this);
        }
    }
}
