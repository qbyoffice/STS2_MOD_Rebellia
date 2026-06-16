using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class CrimsonStrikeDamagePower : RebelliaPowers
{
    private bool _used;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.ExtraHoverTips];
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

    public void SetSourceCard(CardModel source)
    {
        GetData().SourceCard = source;
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        var data = GetData();

        if (command.Attacker != Owner)
            return Task.CompletedTask;
        if (command.ModelSource is not CardModel cardSource)
            return Task.CompletedTask;
        if (cardSource.Type != CardType.Attack)
            return Task.CompletedTask;
        if (cardSource.Tags.Contains(CardTagExtensions.RebelliaBloodWeaponArt))
            return Task.CompletedTask;
        if (cardSource == data.SourceCard)
            return Task.CompletedTask;
        if (data.CommandToModify != null)
            return Task.CompletedTask;

        data.CommandToModify = command;
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (dealer != Owner)
            return 0m;
        if (!props.IsPoweredAttack())
            return 0m;
        if (_used)
            return 0m;

        if (cardSource != null)
        {
            if (cardSource == GetData().SourceCard)
                return 0m;
            if (cardSource.Tags.Contains(CardTagExtensions.RebelliaBloodWeaponArt))
                return 0m;
        }

        return Amount;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        var data = GetData();
        if (command != data.CommandToModify)
            return;

        if (_used)
            return;
        _used = true;

        var strikePower = Owner.GetPower<CrimsonStrikePower>();
        if (strikePower != null)
            await PowerCmd.Remove(strikePower);
        await PowerCmd.Remove(this);
    }

    private class Data
    {
        public AttackCommand? CommandToModify;
        public CardModel? SourceCard;
    }
}
