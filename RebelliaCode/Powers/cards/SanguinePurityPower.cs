using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class SanguinePurityPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;

        await BloodSwordArtManager.AddPoints(Owner, 1, choiceContext);

        if (cardPlay.Card.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
        {
            var veilPower = Owner.GetPower<CrimsonVeilPower>();
            veilPower?.AddVeilPoints(-1);

            cardPlay.Card.RemoveKeyword(RCardKeywordExtensions.RebelliaSanguine);
        }

        await Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (card.Owner != Owner.Player)
            return false;

        if (card.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
        {
            modifiedCost = Math.Max(0, originalCost - 1);
            return true;
        }

        return false;
    }
}
