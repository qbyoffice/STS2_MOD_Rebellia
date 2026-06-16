using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Api.Powers;

public static class BloodBladeTemperManager
{
    public static async Task ApplyTemperCard(
        PlayerChoiceContext context,
        Creature owner,
        CardModel? source,
        bool isUpgraded
    )
    {
        var existingBase = owner.GetPower<BloodBladeTemperPower>();
        var existingUpgraded = owner.GetPower<BloodBladeTemperUpgradedPower>();

        if (isUpgraded)
        {
            if (existingUpgraded != null)
                return;

            int inheritedCount = 0;
            if (existingBase != null)
            {
                inheritedCount = existingBase.GetLostLifeCount();
                await PowerCmd.Remove(existingBase);
            }

            var upgraded = await PowerCmd.Apply<BloodBladeTemperUpgradedPower>(
                context,
                owner,
                1,
                owner,
                source
            );
            if (upgraded != null)
                upgraded.SetLostLifeCount(inheritedCount);
        }
        else
        {
            if (existingUpgraded != null)
                return;
            if (existingBase != null)
                return;

            await PowerCmd.Apply<BloodBladeTemperPower>(context, owner, 1, owner, source);
        }
    }
}
