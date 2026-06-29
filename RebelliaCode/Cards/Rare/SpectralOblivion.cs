using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class SpectralOblivion()
    : RebelliaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<BloodSwordArtPower>(3),
            new CalculationBaseVar(0),
            new ExtraDamageVar(1),
            new CalculatedDamageVar(ValueProp.Unpowered | ValueProp.Unblockable).WithMultiplier(
                (card, target) =>
                {
                    var tempHpPower = card.Owner.Creature.GetPower<RebelliaTmepHpPower>();
                    return tempHpPower?.GetCurrentTempHp() ?? 0m;
                }
            ),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = play.Target;
        if (target == null)
            return;

        var tempHpPower = Owner.Creature.GetPower<RebelliaTmepHpPower>();
        var currentTempHp = tempHpPower!.GetCurrentTempHp();
        var damage = DynamicVars.CalculatedDamage.Calculate(target);
        if (currentTempHp > 0)
            tempHpPower?.AddTempHp(-currentTempHp);

        var mainResult = await DamageCmd
            .Attack(damage!)
            .FromCard(this)
            .Targeting(play.Target!)
            .Execute(choiceContext);

        var unkillDamage = mainResult.Results.SelectMany(list => list).Sum(r => r.OverkillDamage);

        if (unkillDamage <= 0)
            return;

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            await CreatureCmd.Heal(Owner.Creature, currentTempHp);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }
}
