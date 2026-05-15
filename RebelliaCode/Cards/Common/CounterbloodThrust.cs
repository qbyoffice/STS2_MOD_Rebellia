using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Common;

public class CounterbloodThrust()
    : RebelliaCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int RequiredBloodArtPointsValue = 1;

    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Strike, CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(9, ValueProp.Move),
            new IntVar("RequiredBloodArtPoints", RequiredBloodArtPointsValue),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null || play.Target == null)
            return;

        var mainCmd = await CommonActions.CardAttack(this, play).Execute(choiceContext);
        bool brokeBlock = mainCmd.Results.Any(r => r.UnblockedDamage > 0 || r.OverkillDamage > 0);
        if (!brokeBlock)
            return;

        bool hasBlood =
            Owner.Creature.GetPower<BloodSwordArtPower>()?.GetPoints()
            >= RequiredBloodArtPointsValue;
        if (hasBlood)
        {
            if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, RequiredBloodArtPointsValue))
            {
                foreach (var enemy in combatState.HittableEnemies)
                {
                    var aoeCmd = DamageCmd
                        .Attack(DynamicVars.Damage.BaseValue)
                        .FromCard(this)
                        .Targeting(enemy);
                    await aoeCmd.Execute(choiceContext);
                }
            }
        }
        else
        {
            if (combatState.HittableEnemies.Count > 0)
            {
                var randomCmd = DamageCmd
                    .Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .TargetingRandomOpponents(combatState);
                await randomCmd.Execute(choiceContext);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
