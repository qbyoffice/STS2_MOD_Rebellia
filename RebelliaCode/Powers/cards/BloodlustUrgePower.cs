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
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodlustUrgePower : RebelliaPowers
{
    private int _attacksPlayedThisTurn;
    private int _costReduction;
    private int _damageOnFail;
    private int _requiredAttacks;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override int DisplayAmount => 0;
    public override bool ShouldReceiveCombatHooks => true;

    public void SetParameters(int costReduction, int requiredAttacks, int damageOnFail)
    {
        _costReduction = costReduction;
        _requiredAttacks = requiredAttacks;
        _damageOnFail = damageOnFail;
    }

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (_costReduction <= 0)
            return false;

        var isBloodCard =
            card.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon)
            || card.Tags.Contains(CardTagExtensions.RebelliaBloodWeaponArt);
        if (isBloodCard && originalCost > 0)
        {
            modifiedCost = Math.Max(0, originalCost - _costReduction);
            return true;
        }

        return false;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack)
            _attacksPlayedThisTurn++;
        await Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (Owner?.Side != side)
            return;
        if (Owner.Player == null)
            return;

        if (_attacksPlayedThisTurn < _requiredAttacks && _damageOnFail > 0)
            await CreatureCmd.Damage(
                choiceContext,
                Owner,
                _damageOnFail,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
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
