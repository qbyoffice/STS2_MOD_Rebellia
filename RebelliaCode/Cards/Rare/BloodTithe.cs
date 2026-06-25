using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class BloodTithe() : RebelliaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private int _incrementTmepHp;
    private int _currentTmepHpGain = 9;

    [SavedProperty]
    public int CurrentTmepHpGain
    {
        get { return _currentTmepHpGain; }
        set
        {
            AssertMutable();
            _currentTmepHpGain = value;
            DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).BaseValue =
                _currentTmepHpGain;
        }
    }

    [SavedProperty]
    public int IncrementTmepHp
    {
        get { return _incrementTmepHp; }
        set
        {
            AssertMutable();
            _incrementTmepHp = value;
        }
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(9m, ValueProp.Move),
            new PowerVar<RebelliaTmepHpPower>(CurrentTmepHpGain),
            new IntVar("Increment", 3m),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        var tempHpGain = (int)
            DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).BaseValue;
        var tempPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner.Creature);
        tempPower?.AddTempHp(tempHpGain);

        int intIncrementValue = DynamicVars["Increment"].IntValue;
        BuffFromPlay(intIncrementValue);
        (DeckVersion as BloodTithe)?.BuffFromPlay(intIncrementValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Increment"].UpgradeValueBy(1m);
    }

    protected override void AfterDowngraded()
    {
        UpdateCurrentGain();
    }

    private void BuffFromPlay(int extraTmepHp)
    {
        IncrementTmepHp += extraTmepHp;
        UpdateCurrentGain();
    }

    private void UpdateCurrentGain()
    {
        CurrentTmepHpGain = 9 + IncrementTmepHp;
    }
}
