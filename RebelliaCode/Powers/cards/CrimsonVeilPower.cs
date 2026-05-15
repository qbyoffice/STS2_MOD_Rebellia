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
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.DynamicVars;
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
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CrimsonVeilPower>(1)];

    public void AddVeilPoints(int amount)
    {
        var data = GetInternalData<Data>();
        data.VeilPoints = Math.Max(0, data.VeilPoints + amount);
        InvokeDisplayAmountChanged();

        if (data.VeilPoints == 0)
        {
            TaskHelper.RunSafely(PowerCmd.Remove(this));
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

        var hand = PileType.Hand.GetPile(player).Cards;
        var draw = PileType.Draw.GetPile(player).Cards;
        var statusCards = hand.Concat(draw).Where(c => c.Type == CardType.Status).ToList();

        if (statusCards.Count == 0)
            return;

        var randomCard = player.RunState.Rng.CombatCardSelection.NextItem(statusCards);
        if (randomCard == null)
            return;

        bool canPlay = Hook.ShouldPlay(combatState, randomCard, out _, AutoPlayType.Default);
        if (canPlay)
        {
            await CardCmd.AutoPlay(new BlockingPlayerChoiceContext(), randomCard, null);
        }
        else
        {
            await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), randomCard);
        }
    }

    public int GetVeilPoints() => GetInternalData<Data>().VeilPoints;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await TryPlayOrExhaustStatusCard();
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner == null || Owner.Side != side)
            return;

        int veilCount = GetVeilPoints();
        if (veilCount <= 0)
            return;

        decimal conversionRate = DynamicVarsHelper
            .GetPowerVar<CrimsonVeilPower>(DynamicVars)
            .BaseValue;
        int totalToAdd = (int)(veilCount * conversionRate);
        if (totalToAdd > 0)
        {
            var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner);
            if (bloodPower != null)
            {
                int currentBlood = bloodPower.GetPoints();
                int maxBlood = bloodPower.BloodArtMaxPoints;
                int toAdd = Math.Min(totalToAdd, maxBlood - currentBlood);
                if (toAdd > 0)
                    bloodPower.AddPoints(toAdd);
            }
        }

        AddVeilPoints(-1);
    }
}
