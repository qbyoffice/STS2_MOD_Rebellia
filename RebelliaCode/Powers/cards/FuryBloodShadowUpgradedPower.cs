using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers;

public class FuryBloodShadowUpgradedPower : RebelliaPowers, IHasSecondAmount
{
    private int _bloodWeaponsConsumedThisTurn;
    private const int TriggerThreshold = 3;
    protected int BonusPerTrigger = 4;

    public override int DisplayAmount => _bloodWeaponsConsumedThisTurn;

    public string GetSecondAmount() => BonusPerTrigger.ToString();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    public int GetConsumedCount() => _bloodWeaponsConsumedThisTurn;

    public void SetConsumedCount(int value) => _bloodWeaponsConsumedThisTurn = value;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (player.Creature == Owner)
        {
            _bloodWeaponsConsumedThisTurn = 0;
            InvokeDisplayAmountChanged();
        }
        await Task.CompletedTask;
    }

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (card.Owner != Owner.Player)
            return;

        bool isBloodWeapon = card.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon);

        if (isBloodWeapon)
        {
            await CardCmd.Exhaust(choiceContext, card, causedByEthereal: false);
        }

        if (isBloodWeapon)
        {
            _bloodWeaponsConsumedThisTurn++;
            int triggerCount = _bloodWeaponsConsumedThisTurn / TriggerThreshold;
            if (triggerCount > 0)
            {
                int totalBonus = triggerCount * BonusPerTrigger;
                await AddBloodShadowLayers(totalBonus);
                _bloodWeaponsConsumedThisTurn %= TriggerThreshold;
            }
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal
    )
    {
        if (card.Owner != Owner.Player)
            return;

        if (card.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
        {
            card.RemoveKeyword(RCardKeywordExtensions.RebelliaSanguine);
        }
        await Task.CompletedTask;
    }

    private async Task AddBloodShadowLayers(int amount)
    {
        var bloodShadow = Owner.GetPower<BloodShadow>();
        if (bloodShadow != null)
        {
            await PowerCmd.ModifyAmount(
                new BlockingPlayerChoiceContext(),
                bloodShadow,
                amount,
                null,
                null
            );
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        _bloodWeaponsConsumedThisTurn = 0;
        await base.AfterCombatEnd(room);
    }
}
