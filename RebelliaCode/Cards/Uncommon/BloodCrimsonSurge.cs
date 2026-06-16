using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class BloodCrimsonSurge()
    : RebelliaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    private const string PreviewCountKey = "TotalHits";
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.ErodingBlood];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(PreviewCountKey).WithMultiplier(CountStatusCardsWithoutSanguine)
    ];

    private static decimal CountStatusCardsWithoutSanguine(CardModel card, Creature? target)
    {
        var player = card.Owner;
        if (player == null)
            return 0m;

        return PileType
            .Draw.GetPile(player)
            .Cards.Concat(PileType.Discard.GetPile(player).Cards)
            .Concat(PileType.Exhaust.GetPile(player).Cards)
            .Concat(PileType.Hand.GetPile(player).Cards)
            .Count(c =>
                c.Type == CardType.Status
                && !c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine)
            );
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var allCards = PileType
            .Draw.GetPile(Owner)
            .Cards.Concat(PileType.Discard.GetPile(Owner).Cards)
            .Concat(PileType.Exhaust.GetPile(Owner).Cards)
            .Concat(PileType.Hand.GetPile(Owner).Cards);

        var statusCardsToModify = allCards
            .Where(c =>
                c.Type == CardType.Status
                && !c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine)
            )
            .ToList();

        foreach (var card in statusCardsToModify) card.AddKeyword(RCardKeywordExtensions.RebelliaSanguine);

        var addedCount = statusCardsToModify.Count;
        if (addedCount <= 0)
            return;

        var layersPerCard = (int)DynamicVars.CalculationExtra.BaseValue;
        var totalLayers = addedCount * layersPerCard;

        foreach (var enemy in combatState.HittableEnemies)
            await PowerCmd.Apply<ErodingBloodPower>(
                choiceContext,
                enemy,
                totalLayers,
                Owner.Creature,
                this
            );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationExtra.UpgradeValueBy(1);
    }
}