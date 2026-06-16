using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Extensions;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodlustUrgeUpgradedPower : BloodlustUrgePower
{
    private const int COST_REDUCTION = 1;
    private const int DAMAGE_ON_FAIL = 10;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (card.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine) && originalCost > 0)
        {
            modifiedCost = Math.Max(0, originalCost - COST_REDUCTION);
            return true;
        }

        return false;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack)
        {
            await PowerCmd.ModifyAmount(choiceContext, this, -1, null, null);
            if (Amount <= 0)
                await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (Owner?.Side != side || Owner.Player == null)
            return;
        if (Amount > 0)
            await CreatureCmd.Damage(
                choiceContext,
                Owner,
                DAMAGE_ON_FAIL,
                ValueProp.Unpowered | ValueProp.Move,
                null,
                null
            );
        await PowerCmd.Remove(this);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await PowerCmd.Remove(this);
    }
}