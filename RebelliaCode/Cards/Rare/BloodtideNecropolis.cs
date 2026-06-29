using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class BloodtideNecropolis()
    : RebelliaCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(12m),
            new ExtraDamageVar(3m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
                (card, target) =>
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
            ),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        await DamageCmd
            .Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        var handPile = PileType.Hand.GetPile(Owner);
        if (handPile == null || handPile.Cards.Count == 0)
            return;

        foreach (var card in handPile.Cards)
        {
            if (!card.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
            {
                card.AddKeyword(RCardKeywordExtensions.RebelliaSanguine);
            }
        }
        await CardCmd.Discard(choiceContext, handPile.Cards);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(5m);
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}
