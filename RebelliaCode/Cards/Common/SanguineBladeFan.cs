using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Cards.Others;

namespace Rebellia.RebelliaCode.Cards.Common;

public class SanguineBladeFan()
    : RebelliaCard(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.Bloodshiv];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new HpLossVar(2m), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            Owner.Creature,
            this,
            play
        );

        var damageCmd = DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingAllOpponents(combatState);
        await damageCmd.Execute(choiceContext);

        var maxHandSize = 10;

        var baseDart = combatState.CreateCard<DartBloodWeapon>(Owner);
        var currentHandCount = PileType.Hand.GetPile(Owner).Cards.Count;
        var targetPile = currentHandCount < maxHandSize ? PileType.Hand : PileType.Draw;
        var addResult = await CardPileCmd.AddGeneratedCardToCombat(baseDart, targetPile, Owner);
        CardCmd.PreviewCardPileAdd(addResult);

        var enemyCount = combatState.HittableEnemies.Count;
        var cardsPerEnemy = DynamicVars.Cards.IntValue;
        var totalExtraCards = enemyCount * cardsPerEnemy;
        for (var i = 0; i < totalExtraCards; i++)
        {
            var dart = combatState.CreateCard<DartBloodWeapon>(Owner);
            currentHandCount = PileType.Hand.GetPile(Owner).Cards.Count;
            targetPile = currentHandCount < maxHandSize ? PileType.Hand : PileType.Draw;
            addResult = await CardPileCmd.AddGeneratedCardToCombat(dart, targetPile, Owner);
            CardCmd.PreviewCardPileAdd(addResult);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.HpLoss.UpgradeValueBy(-1m);
    }
}
