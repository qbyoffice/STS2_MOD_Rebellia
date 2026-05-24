using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Rebellia.RebelliaCode.Api.Powers
{
    public static class CrimsonVeilPowerManager
    {
        public static async Task TryPlayOrExhaustStatusCard(Player player)
        {
            if (player?.Creature?.CombatState == null)
                return;

            var combatState = player.Creature.CombatState;

            var drawStatus = PileType
                .Draw.GetPile(player)
                .Cards.Where(c => c.Type == CardType.Status && c.Pile?.Type != PileType.Exhaust)
                .ToList();
            var handStatus = PileType
                .Hand.GetPile(player)
                .Cards.Where(c => c.Type == CardType.Status && c.Pile?.Type != PileType.Exhaust)
                .ToList();
            var discardStatus = PileType
                .Discard.GetPile(player)
                .Cards.Where(c => c.Type == CardType.Status && c.Pile?.Type != PileType.Exhaust)
                .ToList();

            var allStatusCards = drawStatus.Concat(handStatus).Concat(discardStatus).ToList();
            if (allStatusCards.Count == 0)
                return;

            var targetCard = allStatusCards.First();

            var canPlay = Hook.ShouldPlay(combatState, targetCard, out _, AutoPlayType.Default);
            if (canPlay)
                await CardCmd.AutoPlay(new BlockingPlayerChoiceContext(), targetCard, null);
            else
                await CardCmd.Discard(new BlockingPlayerChoiceContext(), targetCard);
        }
    }
}
