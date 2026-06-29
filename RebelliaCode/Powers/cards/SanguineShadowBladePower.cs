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
        PowerData.SourceCards.Add(source.CreateClone());//记录不同升级状态
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

        var sourceCards = PowerData.SourceCards;
        var count = Math.Max(Amount, sourceCards.Count);// Amount 是 Power 层数。sourceCards.Count 是实际记录了多少张源卡

        for (var i = 0; i < count; i++)
        {
            var card =
                i < sourceCards.Count
                    ? sourceCards[i].CreateClone()
                    : combatState.CreateCard<SanguineShadowBlade>(player);//没有的话创建默认的

            var addResult = await CardPileCmd.AddGeneratedCardToCombat(//卡加入战斗手牌
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
        public List<CardModel> SourceCards { get; } = [];
    }
}
