using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodSanguineChainPower : RebelliaPowers
{
    private CardModel? _linkedCard;
    private bool _triggered;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;

    public void SetLinkedCard(CardModel card)
    {
        _linkedCard = card;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (_triggered)
            return;
        if (target != Owner)
            return;
        if (result.UnblockedDamage <= 0)
            return;

        if (_linkedCard != null && _linkedCard.Pile?.Type != PileType.Hand)
        {
            await CardPileCmd.Add(_linkedCard, PileType.Hand);
            _triggered = true;
        }

        await PowerCmd.Remove(this);
    }
}
