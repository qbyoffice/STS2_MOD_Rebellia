using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class FinalBloodKiss()
    : RebelliaCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(16m),
            new ExtraDamageVar(2m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
                (card, target) =>
                {
                    var VeilGain = card.Owner.Creature.GetPower<CrimsonVeilPower>();
                    return VeilGain?.GetVeilPoints() ?? 0m;
                }
            ),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        await DamageCmd
            .Attack(DynamicVars.CalculatedDamage)
            .FromCard(this, play)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        var removedVeil = 0;
        var veilPower = Owner.Creature.GetPower<CrimsonVeilPower>();
        if (veilPower != null)
            removedVeil = veilPower.GetVeilPoints();
        if (veilPower != null && removedVeil > 0)
            veilPower.AddVeilPoints(-removedVeil);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
        DynamicVars.CalculationBase.UpgradeValueBy(4m);
    }
}
