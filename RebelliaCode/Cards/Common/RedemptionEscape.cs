using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class RedemptionEscape()
    : RebelliaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new PowerVar<RebelliaTmepHpPower>(5)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int drawCount = (int)DynamicVars.Cards.BaseValue;
        if (drawCount > 0)
            await CardPileCmd.Draw(choiceContext, drawCount, Owner);

        int tempHpGain = (int)
            DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).BaseValue;
        if (tempHpGain > 0)
        {
            var tempPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner.Creature);
            tempPower?.AddTempHp(tempHpGain);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).UpgradeValueBy(3m);
    }
}
