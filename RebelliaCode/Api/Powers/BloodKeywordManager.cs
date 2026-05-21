using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Extensions;

namespace Rebellia.RebelliaCode.Api.Powers
{
    public static class BloodKeywordManager
    {
        // 将弃牌堆和消耗堆中的鲜血卡牌移动到抽牌堆
        public static async Task MoveBloodCardsToDrawPile(Player player)
        {
            if (player?.PlayerCombatState == null)
                return;

            var discard = PileType.Discard.GetPile(player).Cards;
            var exhaust = PileType.Exhaust.GetPile(player).Cards;
            var bloodCards = discard
                .Concat(exhaust)
                .Where(c =>
                    c != null && c.Keywords.Contains(CardKeywordExtensions.RebelliaSanguine)
                )
                .ToList();

            foreach (var card in bloodCards)
            {
                await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Random);
            }
        }

        // 消耗所有鲜血卡牌（从所有牌堆中移除）
        public static async Task ConsumeAllBloodCards(Player player)
        {
            if (player?.PlayerCombatState == null)
                return;

            var allCards = player
                .PlayerCombatState.AllCards.Where(c =>
                    c != null && c.Keywords.Contains(CardKeywordExtensions.RebelliaSanguine)
                )
                .ToList();

            foreach (var card in allCards)
            {
                await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), card);
            }
        }
    }
}
