using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class SanguineBurst()
    : RebelliaCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    private const string HitCountKey = "TotalHits";
    private const string TurnsInHandKey = "TurnsInHand";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodSwordArt, HoverTipsValue.ErodingBlood];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new PowerVar<BloodSwordArtPower>(1),
        new PowerVar<ErodingBloodPower>(1),
        new(TurnsInHandKey, 0m),
        new CalculationBaseVar(1m),
        new CalculationExtraVar(1m),
        new CalculatedVar(HitCountKey).WithMultiplier((card, _) => card.DynamicVars[TurnsInHandKey].BaseValue
        )
    ];

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side == Owner.Creature.Side && Pile?.Type == PileType.Hand)
        {
            var turns = DynamicVars[TurnsInHandKey].BaseValue + 1;
            DynamicVars[TurnsInHandKey].BaseValue = turns;
        }

        await Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var hitCountVar = DynamicVars[HitCountKey] as CalculatedVar;
        var hitCount = (int)(hitCountVar?.Calculate(null) ?? 1m);
        if (hitCount < 1)
            hitCount = 1;

        var damage = DynamicVars.Damage.BaseValue;
        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        var bloodConsumed = await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood);
        var erodingPerHit = (int)
            DynamicVarsHelper.GetPowerVar<ErodingBloodPower>(DynamicVars).BaseValue;

        for (var i = 0; i < hitCount; i++)
        {
            var aliveEnemies = combatState.HittableEnemies;
            if (aliveEnemies.Count == 0)
                break;
            var target = Owner.RunState.Rng.CombatTargets.NextItem(aliveEnemies);
            if (target == null)
                break;

            var damageCmd = DamageCmd.Attack(damage).FromCard(this).Targeting(target);
            await damageCmd.Execute(choiceContext);

            if (bloodConsumed)
                await PowerCmd.Apply<ErodingBloodPower>(
                    choiceContext,
                    target,
                    erodingPerHit,
                    Owner.Creature,
                    this
                );
        }

        DynamicVars[TurnsInHandKey].BaseValue = 0;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVarsHelper.GetPowerVar<ErodingBloodPower>(DynamicVars).UpgradeValueBy(1m);
    }
}