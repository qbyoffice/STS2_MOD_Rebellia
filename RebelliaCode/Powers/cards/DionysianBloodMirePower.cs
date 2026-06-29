using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Cards.Others;

namespace Rebellia.RebelliaCode.Powers.cards;

public class DionysianBloodMirePower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        BloodSwordArtPower.BloodArtOverflow += OnBloodArtOverflow;

        await Task.CompletedTask;
    }

    private async Task OnBloodArtOverflow(Creature creature, int overflowAmount)
    {
        if (creature != Owner)
            return;
        if (overflowAmount <= 0)
            return;

        var player = Owner.Player;
        if (player == null)
            return;
        var combatState = Owner.CombatState;
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

        var bloodWeaponCards = overflowAmount * Amount;
        var rng = player.RunState.Rng.CombatCardGeneration;

        for (var i = 0; i < bloodWeaponCards; i++)
        {
            var prototype = rng.NextItem(bloodWeaponPrototypes);
            var card = combatState.CreateCard(prototype!, player);

            var PreviewCards = await CardPileCmd.AddGeneratedCardToCombat(
                card,
                PileType.Draw,
                player,
                CardPilePosition.Random
            );
            CardCmd.PreviewCardPileAdd(PreviewCards);
        }
    }
}
