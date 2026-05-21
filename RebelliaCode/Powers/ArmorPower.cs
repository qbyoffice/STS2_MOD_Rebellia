using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers;

public class ArmorPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => Amount;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterCardChangedPiles(
        CardModel card,
        PileType oldPile,
        AbstractModel? source
    )
    {
        if (Amount <= 0)
            return;
        if (card.Pile?.Type == PileType.Hand && card.Type == CardType.Status)
        {
            SetAmount(Amount - 1);
            InvokeDisplayAmountChanged();
            await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), card);
        }
    }
}