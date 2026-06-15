using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class SanguineHeartChainPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack)
            return;
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        var target = cardPlay.Target;
        if (target == null)
            return;

        var erosionPower = target.GetPower<ErodingBloodPower>();
        if (erosionPower != null && erosionPower.Amount > 0)
        {
            var combatState = Owner.CombatState;
            if (combatState != null)
            {
                for (int i = 0; i < (int)Amount; i++)
                {
                    await erosionPower.AfterSideTurnStart(
                        target.Side,
                        new List<Creature> { target },
                        combatState
                    );
                }
            }
        }
        await PowerCmd.Remove(this);
    }
}
