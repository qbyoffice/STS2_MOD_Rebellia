using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class PaleBloodRiderPower : RebelliaPowers
{
    private const int DrawCount = 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => DrawCount;
    public override bool ShouldReceiveCombatHooks => true;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Side)
            return;
        if (Owner.Player == null)
            return;

        var player = Owner.Player;

        var allBloodWeapons = PileType
            .Draw.GetPile(player)
            .Cards.Concat(PileType.Hand.GetPile(player).Cards)
            .Concat(PileType.Discard.GetPile(player).Cards)
            .Where(c => c.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon))
            .ToList();

        if (allBloodWeapons.Count == 0)
            return;

        var toPlay = allBloodWeapons
            .OrderBy(_ => player.RunState.Rng.CombatCardSelection.NextInt())
            .Take(DrawCount)
            .ToList();

        foreach (var card in toPlay)
            await CardCmd.AutoPlay(new BlockingPlayerChoiceContext(), card, null);
    }
}
