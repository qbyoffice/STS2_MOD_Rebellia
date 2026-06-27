using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Rebellia.RebelliaCode.Powers.cards;

public class SpectralJourneyPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected virtual int EnergyCost => 1;
    protected virtual int HPLoss => 6;

    public override bool ShouldDraw(Player player, bool fromHandDraw)
    {
        if (fromHandDraw)
            return true;
        if (player != Owner.Player)
            return true;
        Flash();
        return false;
    }

    public override decimal ModifyHpLostAfterOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (!CombatManager.Instance.IsInProgress || target != Owner)
            return amount;

        if (dealer == Owner)
            return amount;

        return amount > 1 ? 1m : amount;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Side)
            return;

        await CreatureCmd.Damage(
            new BlockingPlayerChoiceContext(),
            Owner,
            HPLoss,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            Owner,
            null
        );
        await CardPileCmd.AddGeneratedCardToCombat(
            CombatState.CreateCard<Void>(Owner.Player!),
            PileType.Draw,
            Owner.Player,
            CardPilePosition.Random
        );
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;
        if (cardPlay.Card.Type != CardType.Attack)
            return;

        await PlayerCmd.LoseEnergy(EnergyCost, Owner.Player!);
        await PowerCmd.Decrement(this);
    }
}
