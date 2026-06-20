using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class ArtifactSpiritPower : RebelliaPowers
{
    private bool _eventSubscribed;
    private const int DrawCount = 1;
    private const int BlockPerCard = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;
    public override int DisplayAmount => DrawCount;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (!_eventSubscribed)
        {
            Utils.BloodArtConsumed += OnBloodArtConsumed;
            _eventSubscribed = true;
        }
        await base.AfterApplied(applier, cardSource);
    }

    private async Task OnBloodArtConsumed(Creature creature)
    {
        if (creature != Owner)
            return;
        if (Owner.Player == null)
            return;
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), DrawCount, Owner.Player);
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Side)
            return;
        if (Owner.Player == null)
            return;

        var handCount = PileType.Hand.GetPile(Owner.Player).Cards.Count;
        if (handCount > 0)
        {
            int blockAmount = handCount * BlockPerCard;
            await CreatureCmd.GainBlock(Owner, blockAmount, ValueProp.Unpowered, null);
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (_eventSubscribed)
        {
            Utils.BloodArtConsumed -= OnBloodArtConsumed;
            _eventSubscribed = false;
        }
        await base.AfterCombatEnd(room);
    }
}
