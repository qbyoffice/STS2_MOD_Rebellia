using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Relics;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Relics;

public class LucentCrystal : RebelliaRelics
{
    private const int UpgradeThreshold = 5;

    [SavedProperty]
    public int Rebellia_MonsterCombatWins { get; set; }

    [SavedProperty]
    public int Rebellia_EliteCombatWins { get; set; }

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    public override int DisplayAmount => Rebellia_MonsterCombatWins + Rebellia_EliteCombatWins;

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (room.RoomType == RoomType.Monster)
        {
            Rebellia_MonsterCombatWins++;
            Flash();
            if (Rebellia_MonsterCombatWins >= UpgradeThreshold)
                await RelicCmd.Replace(this, ModelDb.Relic<BloodCrystal>().ToMutable());
        }
        else if (room.RoomType == RoomType.Elite)
        {
            Rebellia_EliteCombatWins++;
            Flash();
            if (Rebellia_EliteCombatWins >= UpgradeThreshold)
                await RelicCmd.Replace(this, ModelDb.Relic<GlimmerCrystal>().ToMutable());
        }
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext ctx,
        CombatSide side,
        CombatState state
    )
    {
        if (side != Owner.Creature.Side || state.RoundNumber != 1)
            return;

        var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner.Creature);
        if (bloodPower == null)
            return;

        if (bloodPower.BloodArtMaxPoints < 2)
            bloodPower.BloodArtMaxPoints = 2;

        int current = bloodPower.GetPoints();
        if (current < 2)
            bloodPower.AddPoints(1);
    }
}
