using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api.Relics;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Relics;

public class LucentCrystal : RebelliaRelics
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<BloodSwordArtPower>(2) };

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext ctx,
        CombatSide side,
        CombatState state
    )
    {
        if (side != Owner.Creature.Side || state.RoundNumber != 1)
            return;

        var power = Owner.Creature.GetPower<BloodSwordArtPower>();
        if (power == null)
        {
            await PowerCmd.Apply<BloodSwordArtPower>(Owner.Creature, 0, Owner.Creature, null);
            power = Owner.Creature.GetPower<BloodSwordArtPower>();
        }
        power?.AddPoints(
            (int)
                (
                    (PowerVar<BloodSwordArtPower>)DynamicVars[typeof(BloodSwordArtPower).Name]
                ).BaseValue
        );
    }
}
