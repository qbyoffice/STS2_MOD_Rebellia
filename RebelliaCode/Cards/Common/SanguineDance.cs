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

public class SanguineDance()
    : RebelliaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int RequiredBloodArtPointsValue = 1;

    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Strike, CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(12m, ValueProp.Move),
            new IntVar("RequiredBloodArtPoints", RequiredBloodArtPointsValue),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        if (await BloodSwordArtManager.TrySpendPoints(Owner.Creature, RequiredBloodArtPointsValue))
        {
            if (Owner.Creature.GetPower<CrimsonVeilPower>() == null)
            {
                await PowerCmd.Apply<CrimsonVeilPower>(Owner.Creature, 1, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
