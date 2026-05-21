using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
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

public class BloodDart() : RebelliaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodSwordArt, HoverTipsValue.BloodDartDiscount];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new PowerVar<BloodSwordArtPower>(1),
        new PowerVar<BloodDartDiscountPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (Owner.Creature.GetPower<BloodDartDiscountPower>() == null)
        {
            var discountAmount = (int)
                DynamicVarsHelper.GetPowerVar<BloodDartDiscountPower>(DynamicVars).BaseValue;
            await PowerCmd.Apply<BloodDartDiscountPower>(
                choiceContext,
                Owner.Creature,
                discountAmount,
                Owner.Creature,
                this
            );
        }

        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}