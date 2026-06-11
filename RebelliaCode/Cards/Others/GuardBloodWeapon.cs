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
public class GuardBloodWeapon() : RebelliaCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Defend, CardTagExtensions.RebelliaBloodWeapon];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.SanguineExtract];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(5, ValueProp.Move), new PowerVar<RebelliaTmepHpPower>(10)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var percent = (int)
            DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).BaseValue;
        if (percent <= 0)
            return;

        var totalTempHpGain = 0;
        foreach (var enemy in combatState.HittableEnemies)
        {
            var eroding = enemy.GetPower<ErodingBloodPower>();
            if (eroding == null || eroding.Amount <= 0)
                continue;

            var currentHp = enemy.CurrentHp;
            var damagePerTurn = (int)Math.Ceiling(currentHp * eroding.Amount / 100.0);
            if (damagePerTurn <= 0)
                continue;

            totalTempHpGain += (int)Math.Ceiling(damagePerTurn * percent / 100.0);
        }

        if (totalTempHpGain <= 0)
            return;

        var tempPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner.Creature);
        tempPower?.AddTempHp(totalTempHpGain);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
