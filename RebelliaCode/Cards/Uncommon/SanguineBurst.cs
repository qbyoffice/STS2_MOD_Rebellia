using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
            new DynamicVar(TurnsInHandKey, 0m),
            new CalculationBaseVar(1m),
            new CalculationExtraVar(1m),
            new CalculatedVar(HitCountKey).WithMultiplier(
                (card, _) =>
                {
                    return card.DynamicVars[TurnsInHandKey].BaseValue;
                }
            ),
        ];

    public override Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (card == this)
        {
            DynamicVars[TurnsInHandKey].BaseValue = 0;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(
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

        var target = play.Target;
        if (target == null)
        {
            var enemies = combatState.HittableEnemies;
            if (enemies.Count == 0)
                return;
            target = Owner.RunState.Rng.CombatTargets.NextItem(enemies);
        }
        if (target == null)
            return;

        var hitCountVar = DynamicVars[HitCountKey] as CalculatedVar;
        int hitCount = (int)(hitCountVar?.Calculate(target) ?? 1m);
        if (hitCount < 1)
            hitCount = 1;

        decimal damage = DynamicVars.Damage.BaseValue;

        for (int i = 0; i < hitCount; i++)
        {
            var cmd = DamageCmd.Attack(damage).FromCard(this).Targeting(target);
            await cmd.Execute(choiceContext);
        }

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
        {
            int erodingPerHit = (int)
                DynamicVarsHelper.GetPowerVar<ErodingBloodPower>(DynamicVars).BaseValue;
            for (int i = 0; i < hitCount; i++)
            {
                await PowerCmd.Apply<ErodingBloodPower>(
                    choiceContext,
                    target,
                    erodingPerHit,
                    Owner.Creature,
                    this
                );
            }
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVarsHelper.GetPowerVar<ErodingBloodPower>(DynamicVars).UpgradeValueBy(1m);
    }
}
