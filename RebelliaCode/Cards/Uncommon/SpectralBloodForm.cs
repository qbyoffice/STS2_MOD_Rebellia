using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class SpectralBloodForm()
    : RebelliaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<SpectralBloodFormPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Utils.GivePower<SpectralBloodFormPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVarsHelper.GetPowerVar<SpectralBloodFormPower>(DynamicVars).UpgradeValueBy(1);
    }
}
