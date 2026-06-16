using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api.Cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class Cultivate() : RebelliaCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(3), new EnergyVar(1), new IntVar("DiscardCards", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawCount = DynamicVars.Cards.BaseValue;
        await CardPileCmd.Draw(choiceContext, drawCount, Owner);

        var maxDiscard = (int)DynamicVars["DiscardCards"].BaseValue;
        var handPile = PileType.Hand.GetPile(Owner);
        if (handPile.Cards.Count > 0 && maxDiscard > 0)
        {
            var actualMax = Math.Min(maxDiscard, handPile.Cards.Count);
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, actualMax);
            var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);
            var toDiscard = selected.ToList();
            var discardedCount = toDiscard.Count;
            if (discardedCount > 0)
            {
                await CardCmd.Discard(choiceContext, toDiscard);
                var energyPerDiscard = (int)DynamicVars.Energy.BaseValue;
                var totalEnergy = discardedCount * energyPerDiscard;
                if (totalEnergy > 0)
                    await PlayerCmd.GainEnergy(totalEnergy, Owner);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars["DiscardCards"].UpgradeValueBy(1);
    }
}