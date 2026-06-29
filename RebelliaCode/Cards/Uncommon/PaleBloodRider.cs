using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class PaleBloodRider()
    : RebelliaCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PaleBloodRiderPower>(1), new PowerVar<PaleBloodRiderUpgradedPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (IsUpgraded)
            await Utils.GivePower<PaleBloodRiderUpgradedPower>(choiceContext, this, play);
        else
            await Utils.GivePower<PaleBloodRiderPower>(choiceContext, this, play);
    }
}
