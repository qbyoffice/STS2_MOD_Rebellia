using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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

    private class Data
    {
        public int VeilPoints;
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    private Data GetData()
    {
        return GetInternalData<Data>();
    }

    public void AddVeilPoints(int amount)
    {
        var data = GetData();
        int oldPoints = data.VeilPoints;
        data.VeilPoints = Math.Max(0, data.VeilPoints + amount);
        InvokeDisplayAmountChanged();

        if (amount > 0)
            TaskHelper.RunSafely(BloodKeywordManager.MoveBloodCardsToDrawPile(Owner.Player!));

        if (oldPoints == 0 && data.VeilPoints > 0)
        {
            TaskHelper.RunSafely(CrimsonVeilPowerManager.TryPlayOrExhaustStatusCard(Owner.Player!));
        }

        if (data.VeilPoints == 0)
        {
            if (!Utils.IsBloodConsumptionSuppressed)
                TaskHelper.RunSafely(BloodKeywordManager.ConsumeAllBloodCards(Owner.Player!));
            TaskHelper.RunSafely(PowerCmd.Remove(this));
        }
    }

    public int GetVeilPoints()
    {
        return GetData().VeilPoints;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Side)
            return;

        var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner);
        bloodPower?.AddPoints(1);

        if (combatState.RoundNumber > 1)
        {
            var currentVeil = GetVeilPoints();
            if (currentVeil > 0)
                AddVeilPoints(-1);
        }
    }
}
