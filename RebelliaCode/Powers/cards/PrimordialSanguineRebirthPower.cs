using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Character;

namespace Rebellia.RebelliaCode.Powers.cards;

public class PrimordialSanguineRebirthPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        if (player != Owner.Player)
            return;

        var allCards = ModelDb
            .CardPool<RebelliaCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint);
        var bloodWeaponCards = allCards
            .Where(c => c.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon))
            .ToList();

        if (!bloodWeaponCards.Any())
            return;

        var cards = CardFactory
            .GetDistinctForCombat(
                player,
                bloodWeaponCards,
                Amount,
                player.RunState.Rng.CombatCardGeneration
            )
            .ToList();

        foreach (var card in cards)
        {
            card.SetToFreeThisTurn();
            var result = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
            CardCmd.PreviewCardPileAdd(result);
        }

        Flash();
    }

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (card.Owner != Owner.Player)
            return;
        if (card.Type != CardType.Power)
            return;

        Creature? target = null;
        if (card.TargetType == TargetType.Self)
        {
            target = Owner;
        }
        else if (card.TargetType == TargetType.AnyEnemy)
        {
            var combatState = Owner.CombatState;
            if (combatState != null && combatState.HittableEnemies.Any())
            {
                target = combatState.HittableEnemies.FirstOrDefault();
            }
        }
        await CardCmd.AutoPlay(choiceContext, card, target);
    }
}
