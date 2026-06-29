using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Cards.Common;

namespace Rebellia.RebelliaCode.Powers.cards;

public class SanguineShadowBladePower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    private Data PowerData => GetInternalData<Data>();

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void SetSourceCard(CardModel source)
    {
        PowerData.SourceCard = source;
    }

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

        var count = Amount;

        for (var i = 0; i < count; i++)
        {
            var card =
                PowerData.SourceCard?.CreateClone()
                ?? combatState.CreateCard<SanguineShadowBlade>(player);

            var addResult = await CardPileCmd.AddGeneratedCardToCombat(
                card,
                PileType.Hand,
                player,
                CardPilePosition.Top
            );

            CardCmd.PreviewCardPileAdd(addResult);
        }

        await PowerCmd.Remove(this);
    }

    private class Data
    {
        public CardModel? SourceCard;
    }
}
