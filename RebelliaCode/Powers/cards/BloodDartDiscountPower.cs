using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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
    private bool _CheckedFirstCard;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (card is BloodDart)
        {
            modifiedCost = 0;
            return true;
        }

        return false;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!_CheckedFirstCard)
        {
            _CheckedFirstCard = true;
            if (!(cardPlay.Card is BloodDart))
                await PowerCmd.Remove(this);
        }
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (participants.Contains(Owner))
            _CheckedFirstCard = false;
        await Task.CompletedTask;
    }
}