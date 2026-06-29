using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class BloodCrimsonTremor()
    : RebelliaCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<BloodCrimsonTremorPower>(1),
            new PowerVar<BloodCrimsonTremorUpgradedPower>(1),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (IsUpgraded)
            await Utils.GivePower<BloodCrimsonTremorUpgradedPower>(choiceContext, this, play);
        else
            await Utils.GivePower<BloodCrimsonTremorPower>(choiceContext, this, play);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
