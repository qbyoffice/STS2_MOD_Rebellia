using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class CrimsonVeilPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => GetInternalData<Data>().VeilPoints;
    public override bool ShouldReceiveCombatHooks => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CrimsonVeilPower>(1)];

    protected override object InitInternalData()
    {
        return new Data();
    }

    private Data GetData() => GetInternalData<Data>();

    public void AddVeilPoints(int amount)
    {
        var data = GetData();
        data.VeilPoints = Math.Max(0, data.VeilPoints + amount);
        InvokeDisplayAmountChanged();

        if (amount > 0)
            TaskHelper.RunSafely(BloodKeywordManager.MoveBloodCardsToDrawPile(Owner.Player!));

        if (data.VeilPoints == 0)
        {
            if (!Utils.IsBloodConsumptionSuppressed)
            {
                TaskHelper.RunSafely(BloodKeywordManager.ConsumeAllBloodCards(Owner.Player!));
            }
            TaskHelper.RunSafely(PowerCmd.Remove(this));
        }
    }

    public int GetVeilPoints()
    {
        return GetData().VeilPoints;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner?.Player != null)
            await CrimsonVeilPowerManager.TryPlayOrExhaustStatusCard(Owner.Player);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (Owner == null || Owner.Side != side)
            return;

        var currentVeil = GetVeilPoints();
        if (currentVeil > 0)
            AddVeilPoints(-1);
        var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner);
        if (bloodPower != null)
        {
            var currentBlood = bloodPower.GetPoints();
            var maxBlood = bloodPower.BloodArtMaxPoints;
            if (currentBlood < maxBlood)
                bloodPower.AddPoints(1);
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await PowerCmd.Remove(this);
    }

    private class Data
    {
        public int VeilPoints;
    }
}
