using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class BloodCrimsonDemon()
    : RebelliaCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<BloodCrimsonDemonPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<ArmorPower>(2), new PowerVar<BloodCrimsonDemonPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var armorAmount = (int)DynamicVarsHelper.GetPowerVar<ArmorPower>(DynamicVars).BaseValue;
        await PowerCmd.Apply<ArmorPower>(
            choiceContext,
            Owner.Creature,
            armorAmount,
            Owner.Creature,
            this
        );

        await PowerCmd.Apply<BloodCrimsonDemonPower>(
            choiceContext,
            Owner.Creature,
            DynamicVarsHelper.GetPowerVar<BloodCrimsonDemonPower>(DynamicVars).BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
        EnergyCost.UpgradeBy(-1);
        DynamicVarsHelper.GetPowerVar<ArmorPower>(DynamicVars).UpgradeValueBy(3m);
    }
}
