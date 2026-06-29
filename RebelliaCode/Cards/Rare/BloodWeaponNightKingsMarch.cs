using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Cards.Others;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class BloodWeaponNightKingsMarch()
    : RebelliaCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeapon];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var allCards = PileType
            .Draw.GetPile(Owner)
            .Cards.Concat(PileType.Discard.GetPile(Owner).Cards)
            .Concat(PileType.Exhaust.GetPile(Owner).Cards)
            .Concat(PileType.Hand.GetPile(Owner).Cards)
            .Distinct()
            .ToList();

        var priorityTypes = new[]
        {
            typeof(SmashBloodWeapon),
            typeof(StrikeBloodWeapon),
            typeof(EngageBloodWeapon),
            typeof(SwiftBloodWeapon),
            typeof(DartBloodWeapon),
        };

        var orderedCards = allCards
            .Where(c => priorityTypes.Any(t => t.IsAssignableFrom(c.GetType())))
            .GroupBy(c => c.GetType())
            .OrderBy(g => Array.IndexOf(priorityTypes, g.Key))
            .SelectMany(g => g)
            .ToList();

        if (orderedCards.Count == 0)
            return;

        if (IsUpgraded)
            foreach (var card in orderedCards)
                if (!card.IsUpgraded)
                    CardCmd.Upgrade(card);

        foreach (var card in orderedCards)
            await CardCmd.AutoPlay(choiceContext, card, play.Target);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        AddKeyword(RCardKeywordExtensions.RebelliaSanguine);
    }
}
