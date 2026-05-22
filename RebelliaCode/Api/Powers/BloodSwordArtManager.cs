using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Api.Powers;

public static class BloodSwordArtManager
{
    public static async Task<BloodSwordArtPower> GetOrCreatePower(
        Creature creature,
        PlayerChoiceContext? context = null
    )
    {
        var power = creature.GetPower<BloodSwordArtPower>();
        if (power == null)
        {
            power = await PowerCmd.Apply<BloodSwordArtPower>(context!, creature, 0, creature, null);
            if (power == null)
                throw new InvalidOperationException(
                    $"Failed to create BloodSwordArtPower for {creature}"
                );
        }

        return power;
    }

    public static async Task AddPoints(
        Creature creature,
        int amount,
        PlayerChoiceContext? context = null
    )
    {
        var power = await GetOrCreatePower(creature, context);
        power.AddPoints(amount);
    }

    public static async Task<bool> TrySpendPoints(
        Creature creature,
        int amount,
        PlayerChoiceContext? context = null
    )
    {
        var power = await GetOrCreatePower(creature, context);
        return power.TrySpendPoints(amount);
    }

    public static async Task<int> GetPoints(Creature creature, PlayerChoiceContext? context = null)
    {
        var power = await GetOrCreatePower(creature, context);
        return power.GetPoints();
    }

    public static async Task IncreaseMaxPoints(
        Creature creature,
        int amount,
        PlayerChoiceContext? context = null
    )
    {
        var power = await GetOrCreatePower(creature, context);
        power.BloodArtMaxPoints += amount;
    }
}
