using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class PaleBloodEssence()
    : RebelliaCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.BloodSwordArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("BloodArtMaxPoints", 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var increase = (int)DynamicVars["BloodArtMaxPoints"].BaseValue;
        await BloodSwordArtManager.IncreaseMaxPoints(Owner.Creature, increase, choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BloodArtMaxPoints"].UpgradeValueBy(1m);
    }
}
