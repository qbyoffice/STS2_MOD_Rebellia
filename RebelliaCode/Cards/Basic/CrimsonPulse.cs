
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Basic;

public class CrimsonPulse() : RebelliaCard(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) 
{
    private const int RequiredBloodPoints = 2;

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike,CardTagExtensions.RebelliaSanguinePoint];
    protected override IEnumerable<DynamicVar> CanonicalVars =>[ new DamageVar(6, ValueProp.Move) ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var strikeCard = GetAvailableStrikeCard();
        if (strikeCard == null)
            return;

        var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner.Creature);
        if (bloodPower == null || bloodPower.GetPoints() < RequiredBloodPoints)
            return;
        if (!bloodPower.TrySpendPoints(RequiredBloodPoints))
            return;

        await CardCmd.AutoPlay(choiceContext, strikeCard, play.Target);
    }

    private CardModel? GetAvailableStrikeCard()
    {
        var combatState = Owner?.PlayerCombatState;
        if (combatState == null)
            return null;

        var handCard = combatState.Hand?.Cards?.FirstOrDefault(c => c is RebelliaStrike);
        if (handCard != null)
            return handCard;

        return combatState.DrawPile?.Cards?.FirstOrDefault(c => c is RebelliaStrike);
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
