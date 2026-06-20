using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class BloodBladeTemper()
    : RebelliaCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<BloodBladeTemperPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<BloodBladeTemperPower>(1), new PowerVar<BloodBladeTemperUpgradedPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        /*await BloodBladeTemperManager.ApplyTemperCard(
            choiceContext,
            Owner.Creature,
            this,
            IsUpgraded
        );*/

        if (IsUpgraded)
        {
            await Utils.GivePower<BloodBladeTemperUpgradedPower>(choiceContext, this, play);
        }
        else
        {
            await Utils.GivePower<BloodBladeTemperPower>(choiceContext, this, play);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
        EnergyCost.UpgradeBy(-1);
    }
}
