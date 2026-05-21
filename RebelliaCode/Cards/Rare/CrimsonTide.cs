using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class CrimsonTide()
    : RebelliaCard(4, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    private const string CalculatedVeilGain = "CalculatedVeilGain";

    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodSwordArt, HoverTipsValue.CrimsonVeil];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(20, ValueProp.Move),
            new CalculationBaseVar(1),
            new CalculationExtraVar(0),
            new EnergyVar(1),
            new CalculatedVar(CalculatedVeilGain).WithMultiplier(
                (card, target) =>
                {
                    var combatState = card.Owner.Creature.CombatState;
                    if (combatState == null)
                        return 0;
                    var aliveCount = combatState.HittableEnemies.Count(e => e.IsAlive);
                    var perAlive = (int)card.DynamicVars["CalculationBase"].BaseValue;
                    return aliveCount * perAlive;
                }
            ),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var totalVeilGain = (int)
            ((CalculatedVar)DynamicVars[CalculatedVeilGain]).Calculate(play.Target);
        if (totalVeilGain > 0)
        {
            var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
            veilPower?.AddVeilPoints(totalVeilGain);
        }

        await DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        var currentBloodArtPoints = Owner.Creature.GetPower<BloodSwordArtPower>()?.GetPoints() ?? 0;
        if (currentBloodArtPoints <= 0)
            return;

        var energyPerPoint = (int)DynamicVars["Energy"].BaseValue;
        var totalEnergyGain = currentBloodArtPoints * energyPerPoint;

        var consumed = await Utils.TryConsumeBloodArtPoints(Owner.Creature, currentBloodArtPoints);
        if (consumed && Owner.Creature.Player != null && totalEnergyGain > 0)
            await PlayerCmd.GainEnergy(totalEnergyGain, Owner.Creature.Player);
    }
}
