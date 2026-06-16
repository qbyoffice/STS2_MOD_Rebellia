using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Api.Relics;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Relics;

public class GlimmerCrystal : RebelliaRelics
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner == null)
            return;

        if (Owner.Creature.HasPower<CrimsonVeilPower>())
            await BloodKeywordManager.MoveBloodCardsToDrawPile(Owner);
        else
            await BloodKeywordManager.ConsumeAllBloodCards(Owner);
    }

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
        if (Owner.Creature.HasPower<CrimsonVeilPower>())
            await BloodKeywordManager.MoveBloodCardsToDrawPile(Owner);
        else
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
