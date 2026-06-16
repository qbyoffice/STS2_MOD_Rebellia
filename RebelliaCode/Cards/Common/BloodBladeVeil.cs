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
using Rebellia.RebelliaCode.Powers;
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

        var maxSelect = Math.Min((int)DynamicVars.Cards.BaseValue, sanguineCards.Count);

        if (maxSelect > 0)
        {
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, maxSelect);
            var selected = await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                sanguineCards,
                Owner,
                prefs
            );

            foreach (var card in selected)
                card.RemoveKeyword(RCardKeywordExtensions.RebelliaSanguine);

            var veilGain = (int)
                DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).BaseValue;
            if (!Owner.Creature.HasPower<CrimsonVeilPower>())
            {
                var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
                veilPower?.AddVeilPoints(veilGain);
            }

            var removedCount = selected.Count();
            if (removedCount > 0)
            {
                var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner.Creature);
                bloodPower?.AddPoints(removedCount);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
