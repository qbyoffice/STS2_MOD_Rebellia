using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Api.Relics;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Relics;

public class BloodCrystal : RebelliaRelics
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext ctx,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState state
    )
    {
        if (side != Owner.Creature.Side || state.RoundNumber != 1)
            return;
        await CrimsonVeilPowerManager.TryPlayOrExhaustStatusCard(Owner);
        await BloodKeywordManager.ConsumeAllBloodCards(Owner);
        var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner.Creature);
        if (bloodPower != null)
        {
            bloodPower.BloodArtMaxPoints = 2;
            var current = bloodPower.GetPoints();
            if (current < 2)
                bloodPower.AddPoints(1);
        }

        var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
        veilPower?.AddVeilPoints(1);
    }
}
