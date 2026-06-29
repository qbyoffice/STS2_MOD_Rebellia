using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Cards.Others;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class BubonicPlague() : RebelliaCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var handPile = PileType.Hand.GetPile(Owner);
        var handCards = handPile.Cards.Where(c => c != null).ToList();
        if (handCards.Count == 0)
            return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, handCards.Count);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, handCards, Owner, prefs);
        var selectCards = selected.ToList();
        if (selectCards.Count == 0)
            return;

        foreach (var card in selectCards)
        {
            if (IsUpgraded)
                CardCmd.Upgrade(card);
            await CardCmd.TransformTo<SmashBloodWeapon>(card);
        }

        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
            if (
                !card.EnergyCost.CostsX && card.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon)
            )
                card.SetToFreeThisTurn();
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
