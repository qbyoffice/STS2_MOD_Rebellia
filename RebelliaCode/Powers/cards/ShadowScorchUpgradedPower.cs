using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class ShadowScorchUpgradedPower : RebelliaPowers
{
    private const int DrawCount = 2;
    private const int EnergyGain = 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;
    public override int DisplayAmount => 2;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (Owner != player.Creature)
            return;
        if (Owner.Player == null)
            return;

        await PlayerCmd.GainEnergy(EnergyGain, Owner.Player);

        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), DrawCount, Owner.Player);
    }
}
