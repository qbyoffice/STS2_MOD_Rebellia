using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class HomewardJourney() : RebelliaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new EnergyVar(1), new CardsVar(1), new PowerVar<HomewardJourneyPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var energyGain = (int)DynamicVars.Energy.BaseValue;
        await PlayerCmd.GainEnergy(energyGain, Owner);

        var drawPile = PileType.Draw.GetPile(Owner);
        var attackCards = drawPile.Cards.Where(c => c.Type == CardType.Attack).ToList();
        var selectCount = (int)DynamicVars.Cards.BaseValue;
        if (attackCards.Count > 0 && selectCount > 0)
        {
            var actualSelect = Math.Min(selectCount, attackCards.Count);
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, actualSelect, actualSelect);
            var selected = await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                attackCards,
                Owner,
                prefs
            );
            foreach (var card in selected)
                await CardPileCmd.Add(card, PileType.Hand);
        }

        if (!Owner.Creature.HasPower<HomewardJourneyPower>())
            await Utils.GivePower<HomewardJourneyPower>(choiceContext, this, play);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Eternal);
        AddKeyword(CardKeyword.Innate);
    }
}
