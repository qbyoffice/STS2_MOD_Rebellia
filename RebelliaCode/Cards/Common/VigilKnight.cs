using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class VigilKnight() : RebelliaCard(2, CardType.Skill, CardRarity.Common, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new PowerVar<RebelliaTmepHpPower>(5)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var targetPlayer = cardPlay.Target?.Player;
        if (targetPlayer == null)
            return;

        if (targetPlayer == Owner)
            return;

        var drawCount = (int)DynamicVars.Cards.BaseValue;
        if (drawCount > 0)
            await CardPileCmd.Draw(choiceContext, drawCount, targetPlayer);

        var tempHpGain = (int)
            DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).BaseValue;
        if (tempHpGain > 0)
        {
            var tempPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(
                targetPlayer.Creature
            );
            tempPower?.AddTempHp(tempHpGain);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Cards.UpgradeValueBy(1m);
        DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).UpgradeValueBy(3m);
    }
}