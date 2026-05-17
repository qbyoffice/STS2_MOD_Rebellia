using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class RendPower : RebelliaPowers
{
    private decimal RenddamageValue = 0;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;

    public void SetDamageValue(decimal value) => RenddamageValue = value;

    public override async Task AfterAttack(AttackCommand command)
    {
        if (command.Attacker != Owner)
            return;
        if (RenddamageValue <= 0)
            return;
        await CreatureCmd.Damage(
            new BlockingPlayerChoiceContext(),
            Owner,
            RenddamageValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            null,
            null
        );
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
