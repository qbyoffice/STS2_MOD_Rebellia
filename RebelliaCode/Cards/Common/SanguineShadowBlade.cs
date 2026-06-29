using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class SanguineShadowBlade()
    : RebelliaCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.BloodSwordArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(11m, ValueProp.Move),
            new PowerVar<BloodSwordArtPower>(1),
            new PowerVar<SanguineShadowBladePower>(1),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var powerAmount = DynamicVarsHelper
            .GetPowerVar<SanguineShadowBladePower>(DynamicVars)
            .BaseValue;
        var power = await Utils.ApplyPower<SanguineShadowBladePower>(
            Owner.Creature,
            powerAmount,
            Owner.Creature,
            this,
            context: choiceContext
        );
        power?.SetSourceCard(this);

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;

        if (!await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            return;

        await CommonActions.CardAttack(this, play).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
