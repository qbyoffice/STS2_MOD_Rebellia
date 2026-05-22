using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Others;

public class SwiftBloodWeapon()
    : RebelliaCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeapon];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new PowerVar<RebelliaTmepHpPower>(10)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        int percent = (int)
            DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).BaseValue;
        if (percent > 0)
        {
            int damage = (int)DynamicVars.Damage.BaseValue;
            int tempHpGain = (int)(damage * percent / 100f);
            if (tempHpGain > 0)
            {
                var tempPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(
                    Owner.Creature,
                    0
                );
                tempPower?.AddTempHp(tempHpGain);
            }
        }

        await CommonActions.CardBlock(this, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
