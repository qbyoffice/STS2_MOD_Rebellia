using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodCrimsonLinkPower : RebelliaPowers
{
    private bool _nextAttackFree = true;
    private bool _nextSkillFree = false;

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
        if (_nextAttackFree && card.Type == CardType.Attack)
        {
            modifiedCost = 0;
            return true;
        }
        if (_nextSkillFree && card.Type == CardType.Skill)
        {
            modifiedCost = 0;
            return true;
        }
        return false;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        if (_nextAttackFree && cardPlay.Card.Type == CardType.Attack)
        {
            _nextAttackFree = false;
            _nextSkillFree = true;
        }
        else if (_nextSkillFree && cardPlay.Card.Type == CardType.Skill)
        {
            _nextAttackFree = false;
            _nextSkillFree = false;
        }
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (Owner?.Side == side)
        {
            var dexterityPower = Owner.GetPower<DexterityPower>();
            await PowerCmd.ModifyAmount(
                choiceContext,
                dexterityPower!,
                -Amount * 2,
                null,
                null,
                true
            );

            var strengthPower = Owner.GetPower<StrengthPower>();
            await PowerCmd.ModifyAmount(
                choiceContext,
                strengthPower!,
                -Amount * 2,
                null,
                null,
                true
            );

            await PowerCmd.Remove(this);
        }
    }
}
