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
        var hasBase = owner.HasPower<BloodBladeTemperPower>();
        var hasUpgraded = owner.HasPower<BloodBladeTemperUpgradedPower>();

        if (isUpgraded)
        {
            if (hasUpgraded)
                return;

            if (hasBase)
            {
                var existingBase = owner.GetPower<BloodBladeTemperPower>();
                var inheritedCount = existingBase.GetLostLifeCount();
                await PowerCmd.Remove(existingBase);
                var upgraded = await PowerCmd.Apply<BloodBladeTemperUpgradedPower>(
                    context,
                    owner,
                    1,
                    owner,
                    source
                );
                upgraded?.SetLostLifeCount(inheritedCount);
                return;
            }

            var newUpgraded = await PowerCmd.Apply<BloodBladeTemperUpgradedPower>(
                context,
                owner,
                1,
                owner,
                source
            );
            newUpgraded?.SetLostLifeCount(0);
        }
        else
        {
            if (hasBase || hasUpgraded)
                return;

            var newBase = await PowerCmd.Apply<BloodBladeTemperPower>(
                context,
                owner,
                1,
                owner,
                source
            );
            newBase?.SetLostLifeCount(0);
        }
    }
}
