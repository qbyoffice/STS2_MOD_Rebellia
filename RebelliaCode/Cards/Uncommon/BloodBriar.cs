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

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class BloodBriar()
    : RebelliaCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.BloodSwordArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(5m, ValueProp.Move),
            new HpLossVar(3m),
            new PowerVar<BloodSwordArtPower>(2),
            new RepeatVar(2),
            new CardsVar(1),
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

        var hitCount = DynamicVars.Repeat.IntValue;
        var attackCmd = CommonActions.CardAttack(this, play, hitCount);
        await attackCmd.Execute(choiceContext);

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (!await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            return;

        var hand = PileType.Hand.GetPile(Owner).Cards;
        var draw = PileType.Draw.GetPile(Owner).Cards;
        var discard = PileType.Discard.GetPile(Owner).Cards;
        var allBloodWeaponCards = hand.Concat(draw)
            .Concat(discard)
            .Where(c => c.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon))
            .ToList();

        if (allBloodWeaponCards.Count == 0)
            return;

        var cardCount = DynamicVars.Cards.IntValue;
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var rngCard = Owner.RunState.Rng.CombatCardSelection;
        var rngTarget = Owner.RunState.Rng.CombatTargets;
        var enemies = combatState.HittableEnemies;

        for (var i = 0; i < cardCount && allBloodWeaponCards.Count > 0 && enemies.Count > 0; i++)
        {
            var randomCard = rngCard.NextItem(allBloodWeaponCards);
            if (randomCard == null)
                continue;

            var randomTarget = rngTarget.NextItem(enemies);
            await CardCmd.AutoPlay(choiceContext, randomCard, randomTarget);

            allBloodWeaponCards.Remove(randomCard);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
