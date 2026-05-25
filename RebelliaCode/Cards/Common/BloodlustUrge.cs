using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class BloodlustUrge() : RebelliaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new HpLossVar(15m),
            new CardsVar(6),
            new EnergyVar(1),
            new PowerVar<BloodlustUrgePower>(1),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var damageOnFail = (int)DynamicVars.HpLoss.BaseValue;
        var requiredAttacks = (int)DynamicVars.Cards.BaseValue;
        var costReductionBase = (int)DynamicVars.Energy.BaseValue;
        var layers = (int)DynamicVarsHelper.GetPowerVar<BloodlustUrgePower>(DynamicVars).BaseValue;

        if (!Owner.Creature.HasPower<CrimsonVeilPower>())
            await Utils.GetOrCreatePower<BloodlustUrgePower>(Owner.Creature, layers);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Cards.UpgradeValueBy(-2);
        DynamicVars.HpLoss.UpgradeValueBy(-5);
    }
}
