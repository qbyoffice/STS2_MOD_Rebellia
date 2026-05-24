using System.Linq;
using System.Threading.Tasks;
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
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class CrimsonTide()
    : RebelliaCard(4, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodSwordArt, HoverTipsValue.CrimsonVeil];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(20, ValueProp.Move), new CalculationBaseVar(1), new EnergyVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        int aliveEnemies = combatState.HittableEnemies.Count(e => e.IsAlive);
        int perAlive = (int)DynamicVars["CalculationBase"].BaseValue;
        int veilGain = aliveEnemies * perAlive;
        if (veilGain > 0)
        {
            var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
            veilPower?.AddVeilPoints(veilGain);
        }

        await DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        var bloodPower = Owner.Creature.GetPower<BloodSwordArtPower>();
        int currentBloodPoints = bloodPower?.GetPoints() ?? 0;
        if (currentBloodPoints <= 0)
            return;

        int energyPerPoint = (int)DynamicVars["Energy"].BaseValue;
        int totalEnergyGain = currentBloodPoints * energyPerPoint;

        if (
            await Utils.TryConsumeBloodArtPoints(Owner.Creature, currentBloodPoints)
            && totalEnergyGain > 0
        )
        {
            await PlayerCmd.GainEnergy(totalEnergyGain, Owner.Creature.Player!);
        }
    }
}
