using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Cards.Common;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodDartDiscountPower : RebelliaPowers
{
    private class Data
    {
        public bool? firstCardIsBloodDart = null;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;

    protected override object InitInternalData() => new Data();

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        var data = GetInternalData<Data>();
        if (data.firstCardIsBloodDart == true && card is BloodDart)
        {
            modifiedCost = 0;
            return true;
        }
        return false;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var data = GetInternalData<Data>();
        if (data.firstCardIsBloodDart == null)
        {
            data.firstCardIsBloodDart = cardPlay.Card is BloodDart;
            if (data.firstCardIsBloodDart == false)
            {
                await PowerCmd.Remove(this);
            }
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side == Owner.Side)
        {
            GetInternalData<Data>().firstCardIsBloodDart = null;
        }
        return Task.CompletedTask;
    }
}
