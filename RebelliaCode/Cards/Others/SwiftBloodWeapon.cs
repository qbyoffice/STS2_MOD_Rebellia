using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Others;

[Pool(typeof(TokenCardPool))]
public class SwiftBloodWeapon()
    : RebelliaCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeapon];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.SanguineExtract];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new PowerVar<RebelliaTmepHpPower>(10)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var target = play.Target;
        if (target == null)
            return;

        var eroding = target.GetPower<ErodingBloodPower>();
        if (eroding == null || eroding.Amount <= 0)
            return;

        var currentHp = target.CurrentHp;
        var damagePerTurn = (int)Math.Ceiling(currentHp * eroding.Amount / 100.0);
        if (damagePerTurn <= 0)
            return;

        var percent = (int)
            DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).BaseValue;
        if (percent <= 0)
            return;

        var tempHpGain = (int)Math.Ceiling(damagePerTurn * percent / 100.0);
        if (tempHpGain <= 0)
            return;

        var tempPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner.Creature);
        tempPower?.AddTempHp(tempHpGain);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
