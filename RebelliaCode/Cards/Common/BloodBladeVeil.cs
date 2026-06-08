using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class BloodBladeVeil() : RebelliaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipsValue.CrimsonVeil,
            HoverTipsValue.KeywordSanguine,
            HoverTipsValue.CrimsonVeilTool,
        ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<CrimsonVeilPower>(1), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        var sanguineCards = exhaustPile
            .Cards.Where(c => c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
            .ToList();

        if (sanguineCards.Count == 0)
            return;

        var selectCount = (int)DynamicVars.Cards.BaseValue;
        selectCount = Math.Min(selectCount, sanguineCards.Count);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, selectCount);
        var selected = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            sanguineCards,
            Owner,
            prefs
        );
        var selectedList = selected.ToList();

        if (selectedList.Count == 0)
            return;

        foreach (var card in selectedList)
            card.RemoveKeyword(RCardKeywordExtensions.RebelliaSanguine);

        var bloodGain = selectedList.Count;
        if (bloodGain > 0)
            await BloodSwordArtManager.AddPoints(Owner.Creature, bloodGain);

        var veilGain = (int)DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).BaseValue;
        if (veilGain > 0)
        {
            var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
            veilPower?.AddVeilPoints(veilGain);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
