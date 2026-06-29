using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodForgeArtsPower : RebelliaPowers
{
    private readonly HashSet<CardModel> _freeCards = new();
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => 2;
    public override bool ShouldReceiveCombatHooks => true;

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner)
            return false;
        if (!card.Tags.Contains(CardTagExtensions.RebelliaBloodWeaponArt))
            return false;

        var player = Owner.Player;
        if (player == null)
            return false;

        var currentEnergy = player.PlayerCombatState?.Energy ?? 0;
        var cost = (int)originalCost;
        if (cost <= 0)
            return false;
        if (currentEnergy >= cost)
            return false;

        modifiedCost = 0;
        _freeCards.Add(card);
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner)
            return;
        if (!_freeCards.Remove(card))
            return;

        var cost = card.EnergyCost.Canonical;
        var damage = cost * 2;
        if (damage <= 0)
            return;

        await CreatureCmd.Damage(
            choiceContext,
            Owner,
            damage,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            Owner,
            null
        );
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side == Owner.Side)
            _freeCards.Clear();
        await Task.CompletedTask;
    }
}
