using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class CrimsonPlatePower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (target != Owner || dealer == null)
            return;

        await TriggerBloodpierce(choiceContext, dealer);
    }

    private async Task TriggerBloodpierce(PlayerChoiceContext choiceContext, Creature attacker)
    {
        var erosionPower = attacker.GetPower<ErodingBloodPower>();
        if (erosionPower != null && erosionPower.Amount > 0)
        {
            var combatState = Owner.CombatState;
            if (combatState != null)
            {
                var participants = new List<Creature> { attacker };
                await erosionPower.AfterSideTurnStart(attacker.Side, participants, combatState);
            }
        }
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        await PowerCmd.Remove(this);
    }
}
