using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class PaleWatchPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    protected override object InitInternalData() => new Data();

    private class Data
    {
        public int BloodWeaponCount { get; set; }
    }

    private Data GetData() => GetInternalData<Data>();

    public override async Task BeforeSideTurnEndEarly(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Side)
            return;

        var handPile = PileType.Hand.GetPile(Owner.Player!);
        var count = handPile.Cards.Count(c =>
            c.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon)
        );
        GetData().BloodWeaponCount = count;

        await Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (player != Owner.Player)
            return;

        int count = GetData().BloodWeaponCount;
        if (count > 0)
            await PlayerCmd.GainEnergy(count, player);
        await PowerCmd.Decrement(this);
    }
}
