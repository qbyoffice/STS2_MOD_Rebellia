using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

class SanguinePurity() : RebelliaCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("BloodArtMaxPoints", 1), new PowerVar<SanguinePurityPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var increase = (int)DynamicVars["BloodArtMaxPoints"].BaseValue;
        await BloodSwordArtManager.IncreaseMaxPoints(Owner.Creature, increase, choiceContext);

        if (!Owner.Creature.HasPower<SanguinePurityPower>())
        {
            await Utils.GivePower<SanguinePurityPower>(
                choiceContext,
                Owner.Creature,
                DynamicVars,
                Owner.Creature,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
        EnergyCost.UpgradeBy(-1);
    }
}
