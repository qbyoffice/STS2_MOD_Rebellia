using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api.Relics;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Relics;

public class LucentCrystal : RebelliaRelics
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (Owner != player)
            return;

        var creature = player.Creature;
        var power = creature.GetPower<BloodSwordArtPower>();

        if (power == null)
        {
            await PowerCmd.Apply<BloodSwordArtPower>(creature, 0, creature, null);
            power = creature.GetPower<BloodSwordArtPower>();
            power?.AddPoints(2);
        }
        else
        {
            power.AddPoints(2);
        }
    }
}
