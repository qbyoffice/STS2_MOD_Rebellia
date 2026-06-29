using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class ImpishPrank() : RebelliaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [RCardKeywordExtensions.RebelliaSanguine];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(16m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var allCards = Owner.PlayerCombatState?.AllCards.Where(c => c != null).ToList();
        foreach (var card in allCards!)
        {
            if (card.Pile?.Type == PileType.Exhaust)
                continue;

            if (card.Pile?.Type == PileType.Discard)
                continue;

            if (card.Pile?.Type == PileType.Draw)
                continue;

            await CardCmd.Exhaust(choiceContext, card);
        }

        var AllCards = PileType
            .Draw.GetPile(Owner)
            .Cards.Concat(PileType.Discard.GetPile(Owner).Cards)
            .Concat(PileType.Exhaust.GetPile(Owner).Cards);

        var sanguineCards = AllCards
            .Where(c => c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
            .ToList();
        await CardPileCmd.Add(sanguineCards, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
