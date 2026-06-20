using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Common;

public class BloodBite() : RebelliaCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.ErodingBlood];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<ErodingBloodPower>(7), new RepeatVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        for (int i = 0; i < base.DynamicVars.Repeat.IntValue; i++)
            await Utils.GivePower<ErodingBloodPower>(
                choiceContext,
                play.Target!,
                DynamicVars,
                Owner.Creature,
                this
            );
    }

    protected override void OnUpgrade()
    {
        DynamicVarsHelper.GetPowerVar<ErodingBloodPower>(DynamicVars).UpgradeValueBy(2m);
    }
}
