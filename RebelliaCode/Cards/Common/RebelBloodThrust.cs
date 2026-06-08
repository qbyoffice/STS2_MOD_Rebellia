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
        var brokeBlock = mainResult
            .Results.SelectMany(list => list)
            .Any(r => r.UnblockedDamage > 0 || r.OverkillDamage > 0);
        if (!brokeBlock)
            return;

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        var bloodPower = Owner.Creature.GetPower<BloodSwordArtPower>();
        var hasBlood = bloodPower != null && bloodPower.GetPoints() >= requiredBlood;

        if (hasBlood && await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
        {
            var baseCmd = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this);
            foreach (var enemy in combatState.HittableEnemies)
                await baseCmd.Targeting(enemy).Execute(choiceContext);
        }
        else if (combatState.HittableEnemies.Count > 0)
        {
            await DamageCmd
                .Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingRandomOpponents(combatState)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
