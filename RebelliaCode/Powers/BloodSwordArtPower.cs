using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers;

public class BloodSwordArtPower : RebelliaPowers, IHasSecondAmount
{
    private int _gainedThisTurn;
    private int _spentThisTurn;
    public int BloodArtMaxPoints { get; set; } = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => GetPointsSafe();
    public override bool ShouldReceiveCombatHooks => true;

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DynamicVar("GainedThisTurn", 0);
            yield return new DynamicVar("SpentThisTurn", 0);
            yield return new DynamicVar("MaxPoints", BloodArtMaxPoints);
            yield return new DynamicVar("BloodArtPoints", GetPointsSafe());
            foreach (var v in base.CanonicalVars)
                yield return v;
        }
    }

    public string GetSecondAmount()
    {
        return $"{BloodArtMaxPoints}";
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public static event Func<Creature, int, Task>? BloodArtOverflow;

    public void AddPoints(int amount)
    {
        if (amount <= 0)
            return;
        var data = GetInternalData<Data>();
        var oldPoints = data.BloodArtPoints;
        var overflow = Math.Max(0, oldPoints + amount - BloodArtMaxPoints);
        var newPoints = Math.Min(oldPoints + amount, BloodArtMaxPoints);

        if (newPoints > oldPoints)
            data.BloodArtPoints = newPoints;

        _gainedThisTurn += amount;

        if (DynamicVars.ContainsKey("BloodArtPoints"))
            DynamicVars["BloodArtPoints"].BaseValue = data.BloodArtPoints;

        if (overflow > 0 && BloodArtOverflow != null)
            BloodArtOverflow.Invoke(Owner, overflow);

        InvokeDisplayAmountChanged();
    }

    public bool TrySpendPoints(int amount)
    {
        if (amount <= 0)
            return true;
        var data = GetInternalData<Data>();
        if (data.BloodArtPoints < amount)
            return false;
        data.BloodArtPoints -= amount;
        _spentThisTurn += amount;

        if (DynamicVars.ContainsKey("SpentThisTurn"))
            DynamicVars["SpentThisTurn"].BaseValue = _spentThisTurn;

        DynamicVars["BloodArtPoints"].BaseValue = data.BloodArtPoints;

        InvokeDisplayAmountChanged();
        return true;
    }

    public int GetPoints()
    {
        return GetInternalData<Data>().BloodArtPoints;
    }

    private int GetPointsSafe()
    {
        var data = GetInternalData<Data>();
        return data?.BloodArtPoints ?? 0;
    }

    public int GetGainedThisTurn()
    {
        return _gainedThisTurn;
    }

    public int GetSpentThisTurn()
    {
        return _spentThisTurn;
    }

    public void UpdateMaxPointsDynamicVar()
    {
        if (DynamicVars.ContainsKey("MaxPoints"))
            DynamicVars["MaxPoints"].BaseValue = BloodArtMaxPoints;
        InvokeDisplayAmountChanged();
    }

    public void SetMaxPoints(int newMax)
    {
        BloodArtMaxPoints = newMax;
        if (DynamicVars.ContainsKey("MaxPoints"))
            DynamicVars["MaxPoints"].BaseValue = newMax;
        InvokeDisplayAmountChanged();
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (participants.Contains(Owner))
        {
            _gainedThisTurn = 0;
            _spentThisTurn = 0;

            if (DynamicVars.ContainsKey("GainedThisTurn"))
                DynamicVars["GainedThisTurn"].BaseValue = 0;
            if (DynamicVars.ContainsKey("SpentThisTurn"))
                DynamicVars["SpentThisTurn"].BaseValue = 0;
        }

        await Task.CompletedTask;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        UpdateMaxPointsDynamicVar();
        DynamicVars["BloodArtPoints"].BaseValue = GetPointsSafe();
        await Task.CompletedTask;
    }

    private class Data
    {
        public int BloodArtPoints;
    }
}
