using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodCrimsonFrenzyPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    protected override object InitInternalData()
    {
        return new Data();
    }

    private Data GetData()
    {
        return GetInternalData<Data>();
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var data = GetData();
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner)
            return;

        data.CurrentCard = card;
        data.BonusCardList[card] = 0;
        await Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var data = GetData();
        if (cardPlay.Card.Owner.Creature == Owner)
            data.CurrentCard = null;
        await Task.CompletedTask;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var data = GetData();
        if (!data.IsSubscribed)
        {
            RebelliaTmepHpPower.TempHpLost += OnTempHpLost;
            data.IsSubscribed = true;
        }

        await Task.CompletedTask;
    }

    private async Task OnTempHpLost(Player player, int lostAmount)
    {
        var data = GetData();
        if (player != Owner.Player)
            return;
        if (data.CurrentCard == null)
            return;
        if (lostAmount <= 0)
            return;

        var card = data.CurrentCard;
        if (data.BonusCardList.TryGetValue(card, out var currentBonus))
            data.BonusCardList[card] = currentBonus + lostAmount;
        else
            data.BonusCardList[card] = lostAmount;

        await Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        var data = GetData();
        if (target != Owner)
            return;
        if (dealer != Owner)
            return;
        if (cardSource == null)
            return;
        if (cardSource != data.CurrentCard)
            return;

        var lost = result.UnblockedDamage + result.OverkillDamage;
        if (lost <= 0)
            return;

        if (data.BonusCardList.TryGetValue(cardSource, out var currentBonus))
            data.BonusCardList[cardSource] = currentBonus + lost;
        else
            data.BonusCardList[cardSource] = lost;

        await Task.CompletedTask;
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (dealer != Owner)
            return 0m;
        if (cardSource == null)
            return 0m;
        var data = GetData();
        if (data.BonusCardList.TryGetValue(cardSource, out var bonus))
            return bonus * Amount;
        return 0m;
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (target != Owner)
            return 0m;
        if (cardSource == null)
            return 0m;
        var data = GetData();
        if (data.BonusCardList.TryGetValue(cardSource, out var bonus))
            return bonus * Amount;
        return 0m;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        var data = GetData();
        if (data.IsSubscribed)
        {
            RebelliaTmepHpPower.TempHpLost -= OnTempHpLost;
            data.IsSubscribed = false;
        }

        data.BonusCardList.Clear();
        data.CurrentCard = null;
        await Task.CompletedTask;
    }

    private class Data
    {
        public readonly Dictionary<CardModel, int> BonusCardList = new();
        public CardModel? CurrentCard { get; set; }
        public bool IsSubscribed { get; set; }
    }
}
