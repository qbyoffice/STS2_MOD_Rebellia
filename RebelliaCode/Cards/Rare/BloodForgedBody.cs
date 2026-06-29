using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class BloodForgedBody() : RebelliaCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<BloodForgedBodyPower>(1), new PowerVar<BloodForgedBodyUpgradedPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (IsUpgraded)
        {
            await Utils.GivePower<BloodForgedBodyUpgradedPower>(choiceContext, this, play);
        }
        else
        {
            await Utils.GivePower<BloodForgedBodyPower>(choiceContext, this, play);
        }
    }
}
