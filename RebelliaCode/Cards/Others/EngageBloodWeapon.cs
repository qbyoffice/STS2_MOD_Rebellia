using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Others;

public class EngageBloodWeapon()
    : RebelliaCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeapon];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new PowerVar<ErodingBloodPower>(1), new CardsVar(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        await PowerCmd.Apply<ErodingBloodPower>(
            choiceContext,
            play.Target!,
            (int)DynamicVarsHelper.GetPowerVar<ErodingBloodPower>(DynamicVars).BaseValue,
            Owner.Creature,
            this
        );
        var drawCount = DynamicVars.Cards.IntValue;
        await CardPileCmd.Draw(choiceContext, drawCount, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        EnergyCost.UpgradeBy(-1);
    }
}
