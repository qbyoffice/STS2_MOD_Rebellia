using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
    public override int DisplayAmount => GetInternalData<Data>().RebelliaTempHp;
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

    protected override object InitInternalData()
    {
        return new Data();
    }

    private Data GetData()
    {
        return GetInternalData<Data>();
    }

    private void UpdateTempHpVar()
    {
        DynamicVars[TempHpVarName].BaseValue = GetData().RebelliaTempHp;
    }

    public void AddTempHp(int amount)
    {
        var data = GetData();
        data.RebelliaTempHp += amount;
        UpdateTempHpVar();
        InvokeDisplayAmountChanged();
    }

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
        var actualDamage = penalty - currentTemp;
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

        data.RebelliaTempHp = penalty;
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
        int reduction = spectralPower?.Amount ?? 0;

        int damage = (int)amount;
        int requiredTempHp = Math.Max(0, damage - reduction);

        if (data.RebelliaTempHp >= requiredTempHp)
        {
            data.RebelliaTempHp -= requiredTempHp;
            UpdateTempHpVar();
            InvokeDisplayAmountChanged();

            return Math.Max(0, damage - requiredTempHp);
        }
        else
        {
            int remainingDamage = damage - reduction - data.RebelliaTempHp;
            remainingDamage = Math.Max(0, remainingDamage);
            data.RebelliaTempHp = 0;
            UpdateTempHpVar();
            InvokeDisplayAmountChanged();
            return remainingDamage;
        }
    }

    public int GetCurrentTempHp()
    {
        return GetData().RebelliaTempHp;
    }

    private class Data
    {
        public int RebelliaTempHp;
    }
}
