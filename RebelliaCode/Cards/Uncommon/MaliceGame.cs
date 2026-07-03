using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class MaliceGame() : RebelliaCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [RCardKeywordExtensions.RebelliaSanguine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new HpLossVar(6), new PowerVar<CrimsonVeilPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var hpLoss = (int)DynamicVars.HpLoss.BaseValue;
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            hpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            Owner.Creature,
            this,
            play
        );

        var cardsToPick = (int)DynamicVars.Cards.BaseValue;
        var discardPile = PileType.Discard.GetPile(Owner);
        var availableCards = discardPile.Cards.ToList();

        if (availableCards.Any())
        {
            IEnumerable<CardModel> selectedCards;
            if (availableCards.Count <= cardsToPick)
            {
                selectedCards = availableCards;
            }
            else
            {
                var prefs = new CardSelectorPrefs(SelectionScreenPrompt, cardsToPick, cardsToPick);
                selectedCards = await CardSelectCmd.FromSimpleGrid(
                    choiceContext,
                    availableCards,
                    Owner,
                    prefs
                );
            }

            foreach (var card in selectedCards)
                await CardPileCmd.Add(card, PileType.Hand);
        }

        var requiredVeil = (int)
            DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).BaseValue;
        var veilPower = Owner.Creature.GetPower<CrimsonVeilPower>();
        if (veilPower != null && veilPower.GetVeilPoints() >= requiredVeil)
        {
            veilPower.AddVeilPoints(-requiredVeil);
            await CardPileCmd.Add(this, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
