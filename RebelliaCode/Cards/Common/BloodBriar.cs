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

namespace Rebellia.RebelliaCode.Cards.Common;

public class BloodBriar()
    : RebelliaCard(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.BloodSwordArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(5m, ValueProp.Move),
            new HpLossVar(3m),
            new PowerVar<BloodSwordArtPower>(2),
            new IntVar("HitCount", 2),
            new IntVar("CardCount", 1),
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

        int hitCount = (int)DynamicVars["HitCount"].BaseValue;
        var attackCmd = CommonActions.CardAttack(this, play, hitCount: hitCount);
        await attackCmd.Execute(choiceContext);

        int requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        var hand = PileType.Hand.GetPile(Owner).Cards;
        var bloodWeaponCards = hand.Where(c =>
                c.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon)
            )
            .ToList();

        if (bloodWeaponCards.Count > 0)
        {
            if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            {
                int cardCount = (int)DynamicVars["CardCount"].BaseValue;
                var combatState = Owner.Creature.CombatState;
                if (combatState == null)
                    return;
                var enemies = combatState.HittableEnemies;
                for (
                    int i = 0;
                    i < cardCount && bloodWeaponCards.Count > 0 && enemies.Count > 0;
                    i++
                )
                {
                    var randomCard = Owner.RunState.Rng.CombatCardSelection.NextItem(
                        bloodWeaponCards
                    );
                    if (randomCard != null)
                    {
                        var randomTarget = Owner.RunState.Rng.CombatTargets.NextItem(enemies);
                        await CardCmd.AutoPlay(
                            choiceContext,
                            randomCard,
                            randomTarget,
                            AutoPlayType.Default
                        );
                        bloodWeaponCards = PileType
                            .Hand.GetPile(Owner)
                            .Cards.Where(c =>
                                c.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon)
                            )
                            .ToList();
                    }
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HitCount"].UpgradeValueBy(1m);
        DynamicVars["CardCount"].UpgradeValueBy(1m);
    }
}
