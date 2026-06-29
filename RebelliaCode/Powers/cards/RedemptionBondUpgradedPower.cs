using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

internal class RedemptionBondUpgradedPower : RebelliaPowers
{
    private const int MaxFreeSkills = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => MaxFreeSkills;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override bool ShouldReceiveCombatHooks => true;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (!participants.Contains(Owner))
            return;
        GetInternalData<Data>().SkillsPlayedThisTurn = 0;
        await Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (card.Owner != Owner.Player)
            return false;
        if (card.Type != CardType.Skill)
            return false;
        if (GetInternalData<Data>().SkillsPlayedThisTurn >= MaxFreeSkills)
            return false;

        modifiedCost = 0;
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;
        if (cardPlay.IsAutoPlay)
            return;
        if (cardPlay.Card.Type != CardType.Skill)
            return;

        GetInternalData<Data>().SkillsPlayedThisTurn++;
        await Task.CompletedTask;
    }

    private class Data
    {
        public int SkillsPlayedThisTurn;
    }
}
