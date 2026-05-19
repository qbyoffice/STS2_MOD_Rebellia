using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

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
            await PowerCmd.ModifyAmount(this, -1, null, null);
            await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), card);
        }
    }
}
