using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Relics;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Relics;

public class BloodCrystal : RebelliaRelics
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext ctx,
        CombatSide side,
        CombatState state
    )
    {
        if (side != Owner.Creature.Side || state.RoundNumber != 1)
            return;

        var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner.Creature);
        if (bloodPower != null)
        {
            bloodPower.BloodArtMaxPoints = 2;
            int current = bloodPower.GetPoints();
            if (current < 2)
                bloodPower.AddPoints(1);
        }

        var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
        veilPower?.AddVeilPoints(1);
    }
}
