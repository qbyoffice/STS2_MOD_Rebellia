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

        var candidates = PileType
            .Draw.GetPile(player)
            .Cards.Concat(PileType.Discard.GetPile(player).Cards)
            .Concat(PileType.Exhaust.GetPile(player).Cards)
            .Where(c => c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
            .ToList();

        if (candidates.Count == 0)
            return;

        var count = Math.Min(Amount, candidates.Count);

        var rng = player.RunState.Rng.CombatCardGeneration;
        var selected = candidates.OrderBy(_ => rng.NextInt()).Take(count).ToList();

        foreach (var card in selected)
        {
            card.RemoveFromCurrentPile();
            var result = await CardPileCmd.Add(card, PileType.Hand);
            card.SetToFreeThisTurn();
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
                target = combatState.HittableEnemies.FirstOrDefault();
        }

        await CardCmd.AutoPlay(choiceContext, card, target);
    }
}
