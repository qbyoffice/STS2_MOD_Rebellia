using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace Rebellia.RebelliaCode.Monsters;

public class Outcast : CustomPetModel
{
    public Outcast()
        : base(visibleHp: true) { }

    public override int MinInitialHp => 20;
    public override int MaxInitialHp => 30;

    public override string? CustomDeathSfx => "";
    public override string? CustomAttackSfx => "";

    public override CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller)
    {
        return SetupAnimationState(
            controller,
            idleName: "idle_loop",
            deadName: "die",
            deadLoop: false,
            hitName: "hurt",
            hitLoop: false,
            attackName: "attack",
            attackLoop: false
        );
    }
}
