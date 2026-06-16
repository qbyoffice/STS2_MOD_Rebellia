using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api.Extensions;

namespace Rebellia.RebelliaCode.Api.Powers;

public static class BloodKeywordManager
{
    public static async Task MoveBloodCardsToDrawPile(Player player)
    {
        if (player?.PlayerCombatState == null)
            return;

        var discard = PileType.Discard.GetPile(player).Cards;
        var exhaust = PileType.Exhaust.GetPile(player).Cards;
        var bloodCards = discard
            .Concat(exhaust)
            .Where(c => c != null && c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
            .ToList();

        foreach (var card in bloodCards)
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Random);
    }

    public static async Task ConsumeAllBloodCards(Player player)
    {
        if (player?.PlayerCombatState == null)
            return;

        var allCards = player
            .PlayerCombatState.AllCards.Where(c =>
                c != null && c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine)
            )
            .ToList();

        foreach (var card in allCards)
        {
            if (card.Pile?.Type == PileType.Exhaust)
                continue;

            await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), card);
        }
    }
}