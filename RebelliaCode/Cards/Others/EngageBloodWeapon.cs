using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;

namespace Rebellia.RebelliaCode.Cards.Others;

public class EngageBloodWeapon()
    : RebelliaCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeapon];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new CardsVar(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        var drawCount = DynamicVars.Cards.IntValue;
        await CardPileCmd.Draw(choiceContext, drawCount, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        EnergyCost.UpgradeBy(-1);
    }
}