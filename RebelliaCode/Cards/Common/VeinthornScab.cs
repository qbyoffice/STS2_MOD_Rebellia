using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

class VeinthornScab() : RebelliaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int RequiredBloodArtPointsValue = 1;

    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Strike, CardTagExtensions.RebelliaBloodWeaponArt];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(4m, ValueProp.Move),
            new HpLossVar(3m),
            new IntVar("RequiredBloodArtPoints", RequiredBloodArtPointsValue),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            this
        );

        if (play.Target == null)
            return;
        decimal damageAmount = DynamicVars.Damage.BaseValue;
        await DamageCmd
            .Attack(damageAmount)
            .FromCard(this)
            .Targeting(play.Target)
            .Execute(choiceContext);

        if (await BloodSwordArtManager.TrySpendPoints(Owner.Creature, RequiredBloodArtPointsValue))
        {
            var rendPower = await PowerCmd.Apply<RendPower>(play.Target, 1, Owner.Creature, this);
            rendPower?.SetDamageValue(damageAmount);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
