using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Common;

public class CrimsonLeap() : RebelliaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.KeywordSanguine];
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(2), new EnergyVar(1), new PowerVar<BloodSwordArtPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var drawPile = PileType.Draw.GetPile(Owner);
        var sanguineCards = drawPile
            .Cards.Where(c => c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
            .ToList();

        if (sanguineCards.Count == 0)
            return;

        var targetCount = (int)DynamicVars.Cards.BaseValue;
        var actualCount = Math.Min(targetCount, sanguineCards.Count);
        if (actualCount == 0)
            return;

        List<CardModel> toRemove;

        if (IsUpgraded)
        {
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, actualCount);
            var selected = await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                sanguineCards,
                Owner,
                prefs
            );
            toRemove = selected.ToList();
        }
        else
        {
            var rng = Owner.RunState.Rng.CombatCardSelection;
            toRemove = new List<CardModel>();
            var tempList = new List<CardModel>(sanguineCards);
            for (var i = 0; i < actualCount; i++)
            {
                if (tempList.Count == 0)
                    break;
                var card = rng.NextItem(tempList);
                toRemove.Add(card!);
                tempList.Remove(card!);
            }
        }

        if (toRemove.Count == 0)
            return;

        foreach (var card in toRemove)
            RemoveKeyword(RCardKeywordExtensions.RebelliaSanguine);

        CardCmd.Preview(toRemove, 1.0f);

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
        {
            var totalEnergy = 1 + toRemove.Count;
            await PlayerCmd.GainEnergy(totalEnergy, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
