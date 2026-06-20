using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BloodCrimsonDemonPower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override object InitInternalData() => new Data();

    private class Data
    {
        public int StrengthAddedThisTurn = 0;
        public int DexterityAddedThisTurn = 0;
    }

    private int GetArmorAmount()
    {
        var armor = Owner.GetPower<ArmorPower>();
        return armor?.Amount ?? 0;
    }

    private async Task ApplyTempStrengthDexterity(PlayerChoiceContext? context, int amount)
    {
        if (amount <= 0)
            return;
        var choiceContext = context ?? new ThrowingPlayerChoiceContext();

        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, amount, Owner, null);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, amount, Owner, null);

        var data = GetInternalData<Data>();
        data.StrengthAddedThisTurn = amount;
        data.DexterityAddedThisTurn = amount;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner?.CombatState == null)
            return;
        int armor = GetArmorAmount();
        await ApplyTempStrengthDexterity(null, armor);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Side)
            return;
        int armor = GetArmorAmount();
        await ApplyTempStrengthDexterity(null, armor);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Side)
            return;
        var data = GetInternalData<Data>();

        if (data.StrengthAddedThisTurn > 0)
        {
            var strength = Owner.GetPower<StrengthPower>();
            if (strength != null)
                await PowerCmd.ModifyAmount(
                    choiceContext,
                    strength,
                    -data.StrengthAddedThisTurn,
                    null,
                    null
                );
        }
        if (data.DexterityAddedThisTurn > 0)
        {
            var dexterity = Owner.GetPower<DexterityPower>();
            if (dexterity != null)
                await PowerCmd.ModifyAmount(
                    choiceContext,
                    dexterity,
                    -data.DexterityAddedThisTurn,
                    null,
                    null
                );
        }
        data.StrengthAddedThisTurn = 0;
        data.DexterityAddedThisTurn = 0;
    }
}
