using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;
using Rebellia.RebelliaCode.Relics;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class SanguineWell() : RebelliaCard(3, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<SanguineWellPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<SanguineWellPower>(1),
            new CalculationBaseVar(0m),
            new CalculationExtraVar(1m),
            new DynamicVar("UpgradeBonus", 0m),
            new DynamicVar("Percent", 3m),
            new CalculatedVar("WellLayers").WithMultiplier(
                (card, _) =>
                {
                    var player = card.Owner;
                    if (player == null)
                        return 0m;

                    var crystal = player.GetRelic<LucentCrystal>();
                    if (crystal == null)
                        return 0m;

                    decimal essence = crystal.BloodEssence;
                    decimal bonus = card.DynamicVars["UpgradeBonus"].BaseValue;
                    decimal percent = card.DynamicVars["Percent"].BaseValue;
                    decimal maxHp = player.Creature.MaxHp;

                    decimal layers = (essence + bonus) * (percent / 100m) * maxHp;
                    return Math.Ceiling(layers);
                }
            ),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var calcVar = DynamicVars["WellLayers"] as CalculatedVar;
        if (calcVar == null)
            return;

        int layers = (int)calcVar.Calculate(null);
        if (layers <= 0)
            return;

        await PowerCmd.Apply<SanguineWellPower>(
            choiceContext,
            Owner.Creature,
            layers,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["UpgradeBonus"].UpgradeValueBy(2m);
        DynamicVars["Percent"].UpgradeValueBy(1m);
    }
}
