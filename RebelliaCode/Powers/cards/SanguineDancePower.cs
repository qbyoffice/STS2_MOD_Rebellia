using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class SanguineDancePower : RebelliaPowers
{
    private int SanguineDancedrawCount = 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;

    public void SetDrawCount(int count)
    {
        SanguineDancedrawCount = count;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Card.Tags.Contains(CardTagExtensions.RebelliaBloodWeaponArt))
            return;

        var player = Owner.Player;
        if (player == null)
            return;

        for (var i = 0; i < SanguineDancedrawCount; i++) await DrawRandomCardFromAllPiles(choiceContext, player);
    }

    private async Task DrawRandomCardFromAllPiles(PlayerChoiceContext choiceContext, Player player)
    {
        var drawPile = PileType.Draw.GetPile(player).Cards.ToList();
        var discardPile = PileType.Discard.GetPile(player).Cards.ToList();
        var allCards = drawPile.Concat(discardPile).ToList();

        if (allCards.Count == 0)
            return;

        var randomCard = player.RunState.Rng.CombatCardSelection.NextItem(allCards);
        if (randomCard == null)
            return;

        await CardPileCmd.Add(randomCard, PileType.Hand);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side) await PowerCmd.Remove(this);
    }
}