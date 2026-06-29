using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Api.Powers;

public static class FuryBloodShadowManager
{
    public static async Task ApplyFuryCard(
        PlayerChoiceContext context,
        Creature owner,
        CardModel? source,
        bool isUpgraded
    )
    {
        var bloodShadow = owner.GetPower<BloodShadow>();
        var targetLayers = isUpgraded ? 7 : 5;
        if (bloodShadow != null)
        {
            if (bloodShadow.Amount < targetLayers)
            {
                var diff = targetLayers - bloodShadow.Amount;
                await PowerCmd.ModifyAmount(context, bloodShadow, diff, null, source);
            }
        }
        else
        {
            await PowerCmd.Apply<BloodShadow>(context, owner, targetLayers, owner, source);
        }

        var existingBase = owner.GetPower<FuryBloodShadowPower>();
        var existingUpgraded = owner.GetPower<FuryBloodShadowUpgradedPower>();

        if (isUpgraded)
        {
            if (existingUpgraded != null)
                return;

            var inheritedCount = 0;
            if (existingBase != null)
            {
                inheritedCount = existingBase.GetConsumedCount();
                await PowerCmd.Remove(existingBase);
            }

            var upgraded = await PowerCmd.Apply<FuryBloodShadowUpgradedPower>(
                context,
                owner,
                1,
                owner,
                source
            );
            if (upgraded != null)
                upgraded.SetConsumedCount(inheritedCount);
        }
        else
        {
            if (existingUpgraded != null)
                return;
            if (existingBase != null)
                return;

            await PowerCmd.Apply<FuryBloodShadowPower>(context, owner, 1, owner, source);
        }
    }
}
