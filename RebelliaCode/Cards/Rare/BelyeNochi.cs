using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;

namespace Rebellia.RebelliaCode.Cards.Rare;

internal class BelyeNochi() : RebelliaCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.KeywordSanguine];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var sanguineCards = PileType
            .Exhaust.GetPile(Owner)
            .Cards.Where(c => c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
            .ToList();
        if (sanguineCards.Count == 0)
            return;

        List<CardModel> toRemove;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, sanguineCards.Count);
        var selected = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            sanguineCards,
            Owner,
            prefs
        );
        toRemove = selected.ToList();

        if (toRemove.Count == 0)
            return;

        foreach (var card in toRemove)
        {
            RemoveKeyword(RCardKeywordExtensions.RebelliaSanguine);
            await CommonActions.CardBlock(this, play);
        }

        var totalEnergy = toRemove.Count;
        await PlayerCmd.GainEnergy(totalEnergy, Owner);

        CardCmd.Preview(toRemove, 1.0f);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        RemoveKeyword(CardKeyword.Exhaust);
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
