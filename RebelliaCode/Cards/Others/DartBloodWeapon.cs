using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Others;

public class DartBloodWeapon()
    : RebelliaCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Shiv, CardTagExtensions.RebelliaBloodWeapon];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2m, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        if (play.Target == null)
            return;

        var erosionPower = play.Target.GetPower<ErodingBloodPower>();
        if (erosionPower == null)
        {
            erosionPower = await PowerCmd.Apply<ErodingBloodPower>(
                choiceContext,
                play.Target,
                1,
                Owner.Creature,
                this
            );
        }

        var combatState = Owner.Creature.CombatState;
        if (combatState != null && erosionPower != null && erosionPower.Amount > 0)
        {
            var participants = new List<Creature>();
            await erosionPower.AfterSideTurnStart(play.Target.Side, participants, combatState);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
