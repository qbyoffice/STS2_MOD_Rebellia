using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class SanguineWellPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => Amount;
    public override bool ShouldReceiveCombatHooks => true;

    public override decimal ModifyHpLostBeforeOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (target != Owner)
            return amount;
        if (Amount <= 0)
            return amount;

        var damage = (int)amount;
        var isSelfDamage = cardSource != null && cardSource.Owner == Owner.Player;

        var tempHp = Owner.GetPower<RebelliaTmepHpPower>()?.GetCurrentTempHp() ?? 0;
        var isFatal = damage >= Owner.CurrentHp + tempHp;

        if (isFatal)
        {
            var layers = Amount;
            CreatureCmd.Heal(Owner, layers);
            SetAmount(0);
            PowerCmd.Remove(this);

            return 0;
        }

        if (isSelfDamage)
        {
            var layers = Amount;
            var absorbed = Math.Min(layers, damage);
            if (absorbed > 0)
            {
                SetAmount(layers - absorbed);
                if (Amount <= 0)
                    PowerCmd.Remove(this);
                return damage - absorbed;
            }
        }

        return amount;
    }
}
