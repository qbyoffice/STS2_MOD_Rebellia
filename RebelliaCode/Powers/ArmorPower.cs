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

    public void AddPoints(int amount)
    {
        SetAmount(Amount + amount);
        InvokeDisplayAmountChanged();
    }

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (Amount <= 0)
            return;
        if (card.Pile?.Type == PileType.Hand && card.Type == CardType.Status)
        {
            AddPoints(-1);
            await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), card);
        }
    }
}
