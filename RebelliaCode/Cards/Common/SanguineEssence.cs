using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Common;

public class SanguineEssence() : RebelliaCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<BloodSwordArtPower>(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int amount = (int)DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        await BloodSwordArtManager.AddPoints(Owner.Creature, amount, choiceContext);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(RCardKeywordExtensions.RebelliaSanguine);
        DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).UpgradeValueBy(1m);
    }
}
