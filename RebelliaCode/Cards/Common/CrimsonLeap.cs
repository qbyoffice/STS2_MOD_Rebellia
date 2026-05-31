using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Common;

public class CrimsonLeap() : RebelliaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new EnergyVar(1), new PowerVar<BloodSwordArtPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var drawPile = PileType.Draw.GetPile(Owner);
        var sanguineCards = drawPile
            .Cards.Where(c => c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
            .ToList();

        if (sanguineCards.Count == 0)
            return;

        var removeCount = (int)DynamicVars.Cards.BaseValue;
        removeCount = Math.Min(removeCount, sanguineCards.Count);

        var rng = Owner.RunState.Rng.CombatCardSelection;
        var toRemove = new List<CardModel>();

        for (int i = 0; i < removeCount; i++)
        {
            if (sanguineCards.Count == 0)
                break;
            var card = rng.NextItem(sanguineCards);
            toRemove.Add(card);
            sanguineCards.Remove(card);
        }

        var previewCards = new List<CardModel>(toRemove);

        foreach (var card in toRemove)
            card.RemoveKeyword(RCardKeywordExtensions.RebelliaSanguine);

        if (previewCards.Count > 0)
        {
            CardCmd.Preview(previewCards, time: 1.0f, CardPreviewStyle.HorizontalLayout);
        }

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (!await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            return;

        var energyGain = toRemove.Count * (int)DynamicVars.Energy.BaseValue;
        if (energyGain > 0)
            await PlayerCmd.GainEnergy(energyGain, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
