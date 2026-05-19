using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
        public bool? freeActive = null;
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
        if (card is BloodDart)
        {
            var data = GetInternalData<Data>();
            if (data.freeActive != false)
            {
                modifiedCost = 0;
                return true;
            }
        }
        return false;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var data = GetInternalData<Data>();
        data.freeActive ??= cardPlay.Card is BloodDart;
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        CombatState combatState
    )
    {
        if (side == Owner.Side)
        {
            GetInternalData<Data>().freeActive = null;
        }
        return Task.CompletedTask;
    }
}
