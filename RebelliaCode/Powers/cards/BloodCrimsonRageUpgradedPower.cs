using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

class BloodCrimsonRageUpgradedPower : RebelliaPowers
{
    protected int ArmorAmount = 2;
    private int _lastArmorAmount = 0;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var armor = Owner.GetPower<ArmorPower>();
        if (armor != null)
        {
            armor.DisplayAmountChanged += OnArmorAmountChanged;
            _lastArmorAmount = armor.Amount;
        }
        await base.AfterApplied(applier, cardSource);
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Side)
            return;

        var armor = Owner.GetPower<ArmorPower>();
        if (armor != null)
        {
            armor.AddPoints(ArmorAmount);
        }
        else
        {
            var newArmor = await PowerCmd.Apply<ArmorPower>(
                new BlockingPlayerChoiceContext(),
                Owner,
                ArmorAmount,
                Owner,
                null
            );
            if (newArmor != null)
            {
                newArmor.DisplayAmountChanged += OnArmorAmountChanged;
                _lastArmorAmount = newArmor.Amount;
            }
        }
    }

    private void OnArmorAmountChanged()
    {
        var armor = Owner.GetPower<ArmorPower>();
        if (armor == null)
            return;
        int current = armor.Amount;
        if (current < _lastArmorAmount)
        {
            int decrease = _lastArmorAmount - current;
            if (decrease > 0)
            {
                CardPileCmd.Draw(new BlockingPlayerChoiceContext(), decrease, Owner.Player!);
            }
        }
        _lastArmorAmount = current;
    }
}
