using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class ShadowScorch() : RebelliaCard(3, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<ShadowScorchPower>(1), new PowerVar<ShadowScorchUpgradedPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (IsUpgraded)
            await Utils.GivePower<ShadowScorchUpgradedPower>(choiceContext, this, play);
        else
            await Utils.GivePower<ShadowScorchPower>(choiceContext, this, play);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
        EnergyCost.UpgradeBy(-1);
    }
}
