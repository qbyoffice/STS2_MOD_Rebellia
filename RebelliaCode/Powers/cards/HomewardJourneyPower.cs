using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class HomewardJourneyPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => Amount;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (Owner != player.Creature)
            return;
        if (Amount <= 0)
            return;

        await PlayerCmd.GainEnergy(Amount, player);

        var drawPile = PileType.Draw.GetPile(player);
        var skillCards = drawPile.Cards.Where(c => c.Type == CardType.Skill).ToList();
        if (skillCards.Count == 0)
            await PowerCmd.Remove(this);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, Amount);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, skillCards, player, prefs);
        foreach (var card in selected)
            await CardPileCmd.Add(card, PileType.Hand);

        await PowerCmd.Remove(this);
    }
}