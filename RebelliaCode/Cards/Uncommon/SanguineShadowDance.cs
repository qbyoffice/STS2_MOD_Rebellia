using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api.Cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class SanguineShadowDance()
    : RebelliaCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var Cards = PileType.Discard.GetPile(Owner).Cards.ToList();
        if (Cards.Count == 0)
            return;

        var maxSelect = (int)DynamicVars.Cards.BaseValue;
        maxSelect = Math.Min(maxSelect, Cards.Count);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, maxSelect);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, Cards, Owner, prefs);
        var selectedList = selected.ToList();
        if (selectedList.Count == 0)
            return;

        foreach (var original in selectedList)
        {
            var clone = original.CreateClone();
            clone.AddKeyword(CardKeyword.Exhaust);
            var addResult = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, Owner);
            CardCmd.PreviewCardPileAdd(addResult);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
