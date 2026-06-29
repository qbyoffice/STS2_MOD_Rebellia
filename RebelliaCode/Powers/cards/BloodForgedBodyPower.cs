using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodForgedBodyPower : RebelliaPowers
{
    private int _TempHpGain = 1;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => _TempHpGain * Amount;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterBlockGained(
        Creature creature,
        decimal amount,
        ValueProp props,
        CardModel? cardSource
    )
    {
        if (creature != Owner)
            return;

        var tempHpPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner);
        await tempHpPower!.AddTempHp(_TempHpGain * Amount);
    }
}
