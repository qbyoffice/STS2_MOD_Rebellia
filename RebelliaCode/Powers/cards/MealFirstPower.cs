using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Cards.Others;

namespace Rebellia.RebelliaCode.Powers.cards;

public class MealFirstPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (Owner != player.Creature)
            return;

        var combatState = Owner.CombatState;
        if (combatState == null)
            return;

        int count = (int)Amount;

        for (int i = 0; i < count; i++)
        {
            var bloodclot = combatState.CreateCard<Bloodclot>(player);
            var addResult = await CardPileCmd.AddGeneratedCardToCombat(
                bloodclot,
                PileType.Hand,
                player,
                CardPilePosition.Top
            );
            CardCmd.PreviewCardPileAdd(addResult);
        }

        await PowerCmd.Remove(this);
    }
}
