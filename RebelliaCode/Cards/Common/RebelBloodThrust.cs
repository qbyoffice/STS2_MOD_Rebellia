using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
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

namespace Rebellia.RebelliaCode.Cards.Common;

public class RebelBloodThrust()
    : RebelliaCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.BloodSwordArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new PowerVar<BloodSwordArtPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var mainResult = await CommonActions.CardAttack(this, play).Execute(choiceContext);
        var unblockedDamage = mainResult
            .Results.SelectMany(list => list)
            .Sum(r => r.UnblockedDamage + r.OverkillDamage);

        if (unblockedDamage <= 0)
            return;

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        var consumed = await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood);

        if (consumed)
        {
            await DamageCmd
                .Attack(unblockedDamage)
                .FromCard(this)
                .TargetingAllOpponents(combatState)
                .Execute(choiceContext);
        }
        else
        {
            var enemies = combatState.HittableEnemies;
            if (enemies.Count > 0)
            {
                var rng = Owner.RunState.Rng.CombatTargets;
                var randomEnemy = rng.NextItem(enemies);
                if (randomEnemy != null)
                    await DamageCmd
                        .Attack(unblockedDamage)
                        .FromCard(this)
                        .Targeting(randomEnemy)
                        .Execute(choiceContext);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
