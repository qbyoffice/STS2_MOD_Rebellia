using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Cards.Others;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class RebelArmament() : RebelliaCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<RebelArmamentPower>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<RebelArmamentPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Utils.GivePower<RebelArmamentPower>(choiceContext, this, play);

        var handPile = PileType.Hand.GetPile(Owner);
        var currentCount = handPile.Cards.Count;
        const int maxHandSize = 10;
        var emptySlots = Math.Max(0, maxHandSize - currentCount);
        if (emptySlots <= 0)
            return;

        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var bloodWeaponPrototypes = new CardModel[]
        {
            ModelDb.Card<SmashBloodWeapon>(),
            ModelDb.Card<StrikeBloodWeapon>(),
            ModelDb.Card<EngageBloodWeapon>(),
            ModelDb.Card<SwiftBloodWeapon>(),
            ModelDb.Card<DartBloodWeapon>(),
            ModelDb.Card<GuardBloodWeapon>(),
        };

        var rng = Owner.RunState.Rng.CombatCardSelection;
        for (var i = 0; i < emptySlots; i++)
        {
            var prototype = rng.NextItem(bloodWeaponPrototypes);
            var card = combatState.CreateCard(prototype!, Owner);

            var addResult = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
            CardCmd.PreviewCardPileAdd(addResult);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
