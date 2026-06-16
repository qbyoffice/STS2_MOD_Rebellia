using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class SanguineHeartChain()
    : RebelliaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    private const string ExtraTimesKey = "TotalHits";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodPierce, HoverTipsValue.ErodingBlood];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ErodingBloodPower>(1),
        new PowerVar<SanguineHeartChainPower>(1),
        new CalculationBaseVar(1m),
        new CalculationExtraVar(1m),
        new CalculatedVar(ExtraTimesKey).WithMultiplier((card, _) =>
            {
                var player = card.Owner;
                if (player == null)
                    return 0m;
                var allCards = PileType
                    .Draw.GetPile(player)
                    .Cards.Concat(PileType.Discard.GetPile(player).Cards)
                    .Concat(PileType.Exhaust.GetPile(player).Cards)
                    .Concat(PileType.Hand.GetPile(player).Cards);
                return allCards.Count(c =>
                    c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine)
                );
            }
        )
    ];

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

        var extraVar = DynamicVars[ExtraTimesKey] as CalculatedVar;
        var extraTimes = (int)(extraVar?.Calculate(target) ?? 0m);
        for (var i = 0; i < extraTimes; i++)
            await Utils.GivePower<ErodingBloodPower>(
                choiceContext,
                target,
                DynamicVars,
                Owner.Creature,
                this
            );

        await Utils.GivePower<SanguineHeartChainPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationExtra.UpgradeValueBy(1);
        DynamicVarsHelper.GetPowerVar<SanguineHeartChainPower>(DynamicVars).UpgradeValueBy(1);
    }
}