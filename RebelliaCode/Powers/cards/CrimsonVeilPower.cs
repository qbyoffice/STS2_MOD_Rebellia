using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class CrimsonVeilPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => GetInternalData<Data>().VeilPoints;
    public override bool ShouldReceiveCombatHooks => true;

    protected override object InitInternalData() => new Data();

    private class Data
    {
        public int VeilPoints = 0;
        public bool HasMovedSanguinePoints = false;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CrimsonVeilPower>(1)];

    public void AddVeilPoints(int amount)
    {
        var data = GetInternalData<Data>();
        data.VeilPoints = Math.Max(0, data.VeilPoints + amount);
        InvokeDisplayAmountChanged();

        if (data.VeilPoints == 0)
        {
            TaskHelper.RunSafely(ConsumeAllSanguinePoints());
            TaskHelper.RunSafely(PowerCmd.Remove(this));
        }
    }

    private async Task ConsumeAllSanguinePoints()
    {
        if (Owner == null)
            return;
        var player = Owner.Player;
        if (player == null)
            return;

        var allCards = player.PlayerCombatState?.AllCards ?? Enumerable.Empty<CardModel>();
        var sanguineCards = allCards
            .Where(c => c != null && c.Tags.Contains(CardTagExtensions.RebelliaSanguinePoint))
            .ToList();

        foreach (var card in sanguineCards)
        {
            await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), card);
        }
    }

    private async Task MoveSanguinePointsToDrawPile()
    {
        if (Owner == null)
            return;
        var player = Owner.Player;
        if (player == null)
            return;

        var data = GetInternalData<Data>();
        if (data.HasMovedSanguinePoints)
            return;
        data.HasMovedSanguinePoints = true;

        var allCards = player.PlayerCombatState?.AllCards ?? Enumerable.Empty<CardModel>();
        var sanguineCards = allCards
            .Where(c => c != null && c.Tags.Contains(CardTagExtensions.RebelliaSanguinePoint))
            .ToList();

        foreach (var card in sanguineCards)
        {
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
        }
    }

    private async Task TryPlayOrExhaustStatusCard()
    {
        if (Owner == null)
            return;
        var combatState = Owner.CombatState;
        if (combatState == null)
            return;
        var player = Owner.Player;
        if (player == null)
            return;

        var drawStatus = PileType
            .Draw.GetPile(player)
            .Cards.Where(c =>
                c != null
                && c.Type == CardType.Status
                && c.Pile != null
                && c.Pile.Type != PileType.Exhaust
            )
            .ToList();
        var handStatus = PileType
            .Hand.GetPile(player)
            .Cards.Where(c =>
                c != null
                && c.Type == CardType.Status
                && c.Pile != null
                && c.Pile.Type != PileType.Exhaust
            )
            .ToList();
        var discardStatus = PileType
            .Discard.GetPile(player)
            .Cards.Where(c =>
                c != null
                && c.Type == CardType.Status
                && c.Pile != null
                && c.Pile.Type != PileType.Exhaust
            )
            .ToList();

        var allStatusCards = drawStatus.Concat(handStatus).Concat(discardStatus).ToList();
        if (allStatusCards.Count == 0)
            return;

        var targetCard = allStatusCards.First();

        bool canPlay = Hook.ShouldPlay(combatState, targetCard, out _, AutoPlayType.Default);
        if (canPlay)
        {
            await CardCmd.AutoPlay(new BlockingPlayerChoiceContext(), targetCard, null);
        }
        else
        {
            await CardCmd.Discard(new BlockingPlayerChoiceContext(), targetCard);
        }
    }

    public int GetVeilPoints() => GetInternalData<Data>().VeilPoints;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await TryPlayOrExhaustStatusCard();
        await MoveSanguinePointsToDrawPile();
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner == null || Owner.Side != side)
            return;

        int currentVeil = GetVeilPoints();
        if (currentVeil > 0)
        {
            AddVeilPoints(-1);
        }
        var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner);
        if (bloodPower != null)
        {
            int currentBlood = bloodPower.GetPoints();
            int maxBlood = bloodPower.BloodArtMaxPoints;
            if (currentBlood < maxBlood)
            {
                bloodPower.AddPoints(1);
            }
        }
    }
}
