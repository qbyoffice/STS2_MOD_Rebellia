using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Cards.Others;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class BloodWeaponVault() : RebelliaCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeapon];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodWeaponVaultTool];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new HpLossVar(4m), new IntVar("MaxHandSize", 10), new PowerVar<RebelliaTmepHpPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            this
        );

        var player = Owner;
        var combatState = Owner.Creature.CombatState;
        if (combatState == null || player == null)
            return;

        var maxHandSize = (int)DynamicVars["MaxHandSize"].BaseValue;

        var combos = new List<Func<List<CardModel>>>
        {
            () => CreateMultiple<StrikeBloodWeapon>(2, combatState, player),
            () => CreateMultiple<EngageBloodWeapon>(3, combatState, player),
            () => CreateMultiple<DartBloodWeapon>(4, combatState, player),
            () => CreateMultiple<SmashBloodWeapon>(1, combatState, player),
            () => CreateMultiple<SwiftBloodWeapon>(2, combatState, player),
        };
        var randomCombo = Owner.RunState.Rng.Niche.NextItem(combos);
        if (randomCombo == null)
            return;
        var generatedCards = randomCombo();

        var drawPile = PileType.Draw.GetPile(player).Cards;
        var discardPile = PileType.Discard.GetPile(player).Cards;
        var existingCards = drawPile
            .Concat(discardPile)
            .Where(c => c != this && c.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon))
            .ToList();

        var cardsToAdd = new List<CardModel>();
        cardsToAdd.AddRange(generatedCards);
        cardsToAdd.AddRange(existingCards);

        foreach (var card in cardsToAdd)
        {
            var currentHand = PileType.Hand.GetPile(player).Cards.Count;
            CardPileAddResult addResult;

            if (currentHand < maxHandSize)
            {
                if (existingCards.Contains(card))
                    addResult = await CardPileCmd.Add(card, PileType.Hand);
                else
                    addResult = await CardPileCmd.AddGeneratedCardToCombat(
                        card,
                        PileType.Hand,
                        Owner
                    );
            }
            else
            {
                if (existingCards.Contains(card))
                    addResult = await CardPileCmd.Add(card, PileType.Discard);
                else
                    addResult = await CardPileCmd.AddGeneratedCardToCombat(
                        card,
                        PileType.Discard,
                        Owner
                    );
            }

            CardCmd.PreviewCardPileAdd(addResult);
            await Cmd.Wait(0.05f);
        }

        await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner.Creature);
    }

    private static List<CardModel> CreateMultiple<T>(
        int count,
        ICombatState combatState,
        Player owner
    )
        where T : CardModel
    {
        var cards = new List<CardModel>();
        for (var i = 0; i < count; i++)
            cards.Add(combatState.CreateCard<T>(owner));
        return cards;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HpLoss.UpgradeValueBy(-2m);
        EnergyCost.UpgradeBy(-1);
    }
}
