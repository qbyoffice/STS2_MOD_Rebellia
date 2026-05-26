using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Common;

public class BloodBacklash()
    : RebelliaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const string TotalDamageKey = "TotalDamage";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.BloodSwordArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(7m, ValueProp.Move),
            new PowerVar<BloodSwordArtPower>(2),
            new CalculationBaseVar(1),
            new CalculationExtraVar(0),
            new CalculatedVar(TotalDamageKey).WithMultiplier(
                (card, target) =>
                {
                    var baseDamage = card.DynamicVars.Damage.BaseValue;
                    if (target == null)
                        return baseDamage;

                    var baseExtra = (int)card.DynamicVars.CalculationBase.BaseValue;
                    var extraExtra = (int)card.DynamicVars.CalculationExtra.BaseValue;
                    var extraPerPower = baseExtra + extraExtra;
                    var uniquePowerCount = target.Powers.Select(p => p.Id).Distinct().Count();
                    return baseDamage + uniquePowerCount * extraPerPower;
                }
            ),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = play.Target;
        if (target == null)
            return;

        var consumed = await Utils.TryConsumeBloodArtPoints(
            Owner.Creature,
            (int)DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue
        );

        decimal totalDamage;
        if (consumed)
        {
            var calcVar = DynamicVars[TotalDamageKey] as CalculatedVar;
            totalDamage = calcVar?.Calculate(target) ?? DynamicVars.Damage.BaseValue;
        }
        else
        {
            totalDamage = DynamicVars.Damage.BaseValue;
        }

        var cmd = DamageCmd.Attack(totalDamage).FromCard(this).Targeting(target);
        await cmd.Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(1);
    }
}
