using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class MagicToolFormPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;

    private readonly HashSet<CardModel> _handSanguineCards = new();

    private void UpdateHandSanguineSet(Player player)
    {
        _handSanguineCards.Clear();
        if (player == null)
            return;
        var handPile = PileType.Hand.GetPile(player);
        if (handPile == null)
            return;
        var sanguineCards = handPile.Cards.Where(c =>
            c != null && c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine)
        );
        foreach (var c in sanguineCards)
            _handSanguineCards.Add(c);
    }

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (card.Owner == Owner.Player)
            UpdateHandSanguineSet(Owner.Player);
        await Task.CompletedTask;
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Owner == Owner.Player)
            UpdateHandSanguineSet(Owner.Player);
        await Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side == Owner.Side)
            _handSanguineCards.Clear();
        await Task.CompletedTask;
    }

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal
    )
    {
        if (card.Owner != Owner.Player)
            return;
        if (!_handSanguineCards.Contains(card))
            return;

        await CardCmd.AutoPlay(choiceContext, card, null);
        await BloodKeywordManager.RemoveSanguineFromCard(Owner.Player, card);
        UpdateHandSanguineSet(Owner.Player);
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        UpdateHandSanguineSet(Owner.Player!);
        await Task.CompletedTask;
    }
}
