using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

internal class CrimsonMadmanUpgradedPower : RebelliaPowers
{
    private readonly int _extraDamage = 8;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => _extraDamage * Amount;
    public override bool ShouldReceiveCombatHooks => true;

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
        if (cardSource.Type != CardType.Attack)
            return 0m;
        if (!cardSource.Keywords.Contains(CardKeyword.Exhaust))
            return 0m;

        return _extraDamage * Amount;
    }
}
