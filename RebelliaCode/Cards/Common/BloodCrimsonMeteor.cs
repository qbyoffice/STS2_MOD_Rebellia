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
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Common;

public class BloodCrimsonMeteor()
    : RebelliaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const string TotalHitsKey = "TotalHits";

    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.BloodSwordArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new PowerVar<BloodSwordArtPower>(1),
        new CalculationBaseVar(1),
        new CalculationExtraVar(1),
        new CalculatedVar(TotalHitsKey).WithMultiplier((card, target) =>
            {
                var dex =
                    card.Owner.Creature.GetPowerAmount<DexterityPower>()
                    * card.DynamicVars.CalculationExtra.BaseValue;
                var baseVal = card.DynamicVars.CalculationBase.BaseValue;
                return baseVal + dex - 1;
            }
        )
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        var target = play.Target;
        if (target == null)
            return;

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (!await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            return;

        var dexterity = Owner.Creature.GetPowerAmount<DexterityPower>();
        var extraHits = dexterity;
        var damage = DynamicVars.Damage.BaseValue;

        for (var i = 0; i < extraHits; i++)
            await DamageCmd.Attack(damage).FromCard(this).Targeting(target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}