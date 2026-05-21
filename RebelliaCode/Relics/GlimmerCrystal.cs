using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Api.Relics;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Relics;

public class GlimmerCrystal : RebelliaRelics
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext ctx,
        CombatSide side,
        ICombatState state
    )
    {
        if (side != Owner.Creature.Side || state.RoundNumber != 1)
            return;
        await BloodKeywordManager.ConsumeAllBloodCards(Owner);
        var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner.Creature);
        if (bloodPower != null)
        {
            bloodPower.BloodArtMaxPoints = 3;
            var current = bloodPower.GetPoints();
            var toAdd = 2 - current;
            if (toAdd > 0)
                bloodPower.AddPoints(toAdd);
        }
    }
}
