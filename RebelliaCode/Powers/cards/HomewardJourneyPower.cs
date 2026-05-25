using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.DynamicVars;
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

        int triggers = Amount;
        int energyPerTrigger = (int)DynamicVars.Energy.BaseValue;
        int cardsPerTrigger = (int)DynamicVars.Cards.BaseValue;

        await PowerCmd.Remove(this);

        int totalEnergy = triggers * energyPerTrigger;
        if (totalEnergy > 0)
            await PlayerCmd.GainEnergy(totalEnergy, player);

        int totalCardsToSelect = triggers * cardsPerTrigger;
        if (totalCardsToSelect <= 0)
            return;

        var drawPile = PileType.Draw.GetPile(player);
        var skillCards = drawPile.Cards.Where(c => c.Type == CardType.Skill).ToList();
        if (skillCards.Count == 0)
            return;

        int selectable = System.Math.Min(totalCardsToSelect, skillCards.Count);
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, selectable, selectable);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, skillCards, player, prefs);
        foreach (var card in selected)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }
}
