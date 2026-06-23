using MegaCrit.Sts2.Core.Commands;
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

    [SavedProperty]
    public int ExtraTempHp { get; set; } = 0;

    private int Increment => (int)DynamicVars["Increment"].BaseValue;

    private int CurrentTempHpGain =>
        (int)DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).BaseValue
        + ExtraTempHp;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(9m, ValueProp.Move),
            new IntVar("Increment", 3),
            new PowerVar<RebelliaTmepHpPower>(9),
            new IntVar("TempHpGain", CurrentTempHpGain),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.Damage.BaseValue,
            ValueProp.Move,
            Owner.Creature,
            this
        );

        var tempPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner.Creature);
        if (tempPower == null)
            return;

        int hpGain = CurrentTempHpGain;
        await tempPower.AddTempHp(hpGain);

        ExtraTempHp += Increment;
        DynamicVars["TempHpGain"].BaseValue = CurrentTempHpGain;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Increment"].UpgradeValueBy(1);
        DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).UpgradeValueBy(3m);
    }
}
