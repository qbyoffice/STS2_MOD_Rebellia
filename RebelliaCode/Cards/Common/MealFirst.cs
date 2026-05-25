using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class MealFirst() : RebelliaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new PowerVar<RebelliaTmepHpPower>(1), new PowerVar<MealFirstPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int drawCount = (int)DynamicVars.Cards.BaseValue;
        var drawnCards = await CardPileCmd.Draw(choiceContext, drawCount, Owner);
        if (drawnCards != null)
        {
            foreach (var card in drawnCards)
            {
                if (!card.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
                    card.AddKeyword(RCardKeywordExtensions.RebelliaSanguine);
            }
        }

        await Utils.GivePower<RebelliaTmepHpPower>(choiceContext, this, play);
        await Utils.GivePower<MealFirstPower>(choiceContext, this, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
        EnergyCost.UpgradeBy(-1);
    }
}
