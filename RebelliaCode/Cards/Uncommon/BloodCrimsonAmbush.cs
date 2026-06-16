using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class BloodCrimsonAmbush()
    : RebelliaCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string HitCountKey = "TotalHits";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.BloodSwordArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new CalculationBaseVar(1),
        new CalculationExtraVar(1),
        new CalculatedVar(HitCountKey).WithMultiplier((card, target) =>
            {
                var bloodPower = card.Owner?.Creature?.GetPower<BloodSwordArtPower>();
                if (bloodPower == null)
                    return 0m;
                return bloodPower.GetGainedThisTurn() + bloodPower.GetSpentThisTurn();
            }
        )
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = play.Target;
        if (target == null)
            return;

        var calcVar = DynamicVars[HitCountKey] as CalculatedVar;
        var hitCount = (int)(calcVar?.Calculate(target) ?? 1m);
        if (hitCount < 1)
            hitCount = 1;

        var damage = DynamicVars.Damage.BaseValue;
        var cmd = DamageCmd.Attack(damage).FromCard(this).Targeting(target).WithHitCount(hitCount);
        await cmd.Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}