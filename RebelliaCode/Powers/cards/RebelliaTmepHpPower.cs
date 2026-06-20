using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class RebelliaTmepHpPower : RebelliaPowers
{
    private const string TempHpVarName = "TempHp";
    private bool _isSelfDamage;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => GetData().RebelliaTempHp;
    public override bool ShouldReceiveCombatHooks => true;

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DynamicVar(TempHpVarName, 0);
            foreach (var v in base.CanonicalVars)
                yield return v;
        }
    }

    public static event Func<Player, int, Task>? TempHpLost;

    private static async Task TriggerTempHpLost(Player player, int lostAmount)
    {
        if (TempHpLost != null)
            await TempHpLost.Invoke(player, lostAmount);
    }

    private class Data
    {
        public int RebelliaTempHp { get; set; }
    }

    protected override object InitInternalData() => new Data();

    private Data GetData() => GetInternalData<Data>();

    private void UpdateTempHpVar()
    {
        DynamicVars[TempHpVarName].BaseValue = GetData().RebelliaTempHp;
    }

    public async Task AddTempHp(int amount)
    {
        var data = GetData();
        int oldValue = data.RebelliaTempHp;
        data.RebelliaTempHp += amount;

        if (data.RebelliaTempHp < 0)
            data.RebelliaTempHp = 0;

        int actualLost = oldValue - data.RebelliaTempHp;
        if (actualLost > 0)
        {
            await TriggerTempHpLost(Owner.Player!, actualLost);
        }

        UpdateTempHpVar();
        InvokeDisplayAmountChanged();
    }

    public async Task ReduceTempHp(int amount) => await AddTempHp(-amount);

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Side)
            return;
        var player = Owner.Player;
        if (player == null)
            return;

        var hand = PileType.Hand.GetPile(player).Cards;
        var count = hand.Count(c => c.Tags.Contains(CardTagExtensions.BloodclotExhaust));
        if (count == 0)
            return;

        var penalty = count * 5;
        var data = GetData();
        var currentTemp = data.RebelliaTempHp;
        var actualDamage = Math.Max(0, penalty - currentTemp);
        if (actualDamage > 0)
        {
            _isSelfDamage = true;
            await CreatureCmd.Damage(
                new BlockingPlayerChoiceContext(),
                Owner,
                actualDamage,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                null,
                null
            );
            _isSelfDamage = false;
        }

        int reduceAmount = Math.Max(0, data.RebelliaTempHp - penalty);
        if (reduceAmount > 0)
        {
            await ReduceTempHp(reduceAmount);
        }
        int old = data.RebelliaTempHp;
        data.RebelliaTempHp = penalty;
        int lost = old - penalty;
        if (lost > 0)
        {
            await TriggerTempHpLost(Owner.Player!, lost);
        }
        UpdateTempHpVar();
        InvokeDisplayAmountChanged();
    }

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
        if (_isSelfDamage)
            return amount;

        var data = GetData();
        if (data.RebelliaTempHp <= 0)
            return amount;

        var spectralPower = Owner.GetPower<SpectralBloodFormPower>();
        var reduction = spectralPower?.Amount ?? 0;

        var damage = (int)amount;
        var requiredTempHp = Math.Max(0, damage - reduction);

        if (data.RebelliaTempHp >= requiredTempHp)
        {
            data.RebelliaTempHp -= requiredTempHp;

            Task.Run(async () => await TriggerTempHpLost(Owner.Player!, requiredTempHp));
            UpdateTempHpVar();
            InvokeDisplayAmountChanged();
            return Math.Max(0, damage - requiredTempHp);
        }
        else
        {
            int consumed = data.RebelliaTempHp;
            int remainingDamage = damage - reduction - consumed;
            data.RebelliaTempHp = 0;
            if (consumed > 0)
            {
                Task.Run(async () => await TriggerTempHpLost(Owner.Player!, consumed));
            }
            UpdateTempHpVar();
            InvokeDisplayAmountChanged();
            return Math.Max(0, remainingDamage);
        }
    }

    public int GetCurrentTempHp() => GetData().RebelliaTempHp;
}
