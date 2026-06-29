using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class PaleWatchPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Side)
            return;

        var player = Owner.Player;
        if (player == null)
            return;

        int bloodWeaponCount = PileType
            .Hand.GetPile(player)
            .Cards.Count(c => c.Tags.Contains(CardTagExtensions.RebelliaBloodWeapon));

        await PlayerCmd.GainEnergy(bloodWeaponCount, player);
        await PowerCmd.Decrement(this);
    }
}
