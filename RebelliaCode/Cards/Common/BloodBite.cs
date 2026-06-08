using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class BloodBite() : RebelliaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.ErodingBlood];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<ErodingBloodPower>(7)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
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
