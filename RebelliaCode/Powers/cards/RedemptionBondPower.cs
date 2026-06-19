using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class RedemptionBondPower : RebelliaPowers
{
    private int _freeSkillsRemaining = 0;
    private const int MaxFreeSkills = 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => _freeSkillsRemaining;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _freeSkillsRemaining = MaxFreeSkills;
        await base.AfterApplied(applier, cardSource);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Side)
            return;
        _freeSkillsRemaining = MaxFreeSkills;
        await Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombat(
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
        if (_freeSkillsRemaining <= 0)
            return false;

        modifiedCost = 0;
        _freeSkillsRemaining--;
        return true;
    }
}
