using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

class SteelheartBloodChainUpgradedPower : RebelliaPowers
{
    private const int BlockPerArmor = 5;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => BlockPerArmor;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Side)
            return;

        var armor = Owner.GetPower<ArmorPower>();
        int armorAmount = armor?.Amount ?? 0;
        if (armorAmount <= 0)
            return;

        int blockGain = armorAmount * BlockPerArmor;
        await CreatureCmd.GainBlock(Owner, blockGain, ValueProp.Move, null);
    }
}
