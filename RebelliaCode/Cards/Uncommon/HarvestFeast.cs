using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class HarvestFeast() : RebelliaCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VigorPower>(), HoverTipsValue.RebelliaTempHp];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(11, ValueProp.Move),
            new PowerVar<RebelliaTmepHpPower>(7),
            new PowerVar<VigorPower>(7),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        var tempHpPower = Owner.Creature.GetPower<RebelliaTmepHpPower>();
        if (tempHpPower != null)
        {
            var currentTempHp = tempHpPower.GetCurrentTempHp();
            if (currentTempHp > 0)
            {
                await CreatureCmd.Heal(Owner.Creature, currentTempHp);
                tempHpPower?.AddTempHp(-currentTempHp);
            }
        }

        var tempHpGain = (int)
            DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).BaseValue;
        if (tempHpGain > 0)
        {
            var tempPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner.Creature);
            tempPower?.AddTempHp(tempHpGain);
        }

        await Utils.GivePower<VigorPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).UpgradeValueBy(2m);
        DynamicVarsHelper.GetPowerVar<VigorPower>(DynamicVars).UpgradeValueBy(2m);
    }
}
