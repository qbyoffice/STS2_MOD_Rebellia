using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Api.Powers;

public static class BloodSwordArtManager
{
    public static async Task<BloodSwordArtPower> GetOrCreatePower(Creature creature)
    {
        var power = creature.GetPower<BloodSwordArtPower>();
        if (power == null)
        {
            power = await PowerCmd.Apply<BloodSwordArtPower>(creature, 0, creature, null);
            if (power == null)
                throw new InvalidOperationException(
                    $"Failed to create BloodSwordArtPower for {creature}"
                );
        }
        return power;
    }

    public static async Task AddPoints(Creature creature, int amount)
    {
        var power = await GetOrCreatePower(creature);
        power.AddPoints(amount);
    }

    public static async Task<bool> TrySpendPoints(Creature creature, int amount)
    {
        var power = await GetOrCreatePower(creature);
        return power.TrySpendPoints(amount);
    }

    public static async Task<int> GetPoints(Creature creature)
    {
        var power = await GetOrCreatePower(creature);
        return power.GetPoints();
    }
}
