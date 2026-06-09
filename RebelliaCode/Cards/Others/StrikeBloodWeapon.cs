using BaseLib.Utils;
using Godot;
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

namespace Rebellia.RebelliaCode.Cards.Others;

public class StrikeBloodWeapon()
    : RebelliaCard(1, CardType.Attack, CardRarity.None, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.ErodingBlood];

    protected override HashSet<CardTag> CanonicalTags =>
        [
            CardTag.Strike,
            CardTagExtensions.RebelliaBloodWeapon,
            CardTagExtensions.RebelliaBloodWeaponArt,
        ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(6m, ValueProp.Move),
            new PowerVar<BloodSwordArtPower>(1),
            new PowerVar<ErodingBloodPower>(2),
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            await Utils.GivePower<ErodingBloodPower>(
                choiceContext,
                play.Target!,
                DynamicVars,
                Owner.Creature,
                this
            );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVarsHelper.GetPowerVar<ErodingBloodPower>(DynamicVars).UpgradeValueBy(1m);
    }
}
