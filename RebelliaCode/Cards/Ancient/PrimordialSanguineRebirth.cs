using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Ancient;

public class PrimordialSanguineRebirth()
    : RebelliaCard(3, CardType.Power, CardRarity.Ancient, TargetType.Self),
        ITomeCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PrimordialSanguineRebirthPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Utils.GivePower<PrimordialSanguineRebirthPower>(choiceContext, this, play);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        AddKeyword(CardKeyword.Innate);
    }
}
