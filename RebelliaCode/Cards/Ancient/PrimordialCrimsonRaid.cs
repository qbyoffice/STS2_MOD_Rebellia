using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Ancient;

public class PrimordialCrimsonRaid()
    : RebelliaCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(11m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        int points = Owner.Creature.GetPower<BloodSwordArtPower>()?.GetPoints() ?? 0;
        if (points <= 0)
            return;

        if (!await Utils.TryConsumeBloodArtPoints(Owner.Creature, points))
            return;

        var allCards = PileType
            .Draw.GetPile(Owner)
            .Cards.Concat(PileType.Discard.GetPile(Owner).Cards)
            .Concat(PileType.Exhaust.GetPile(Owner).Cards)
            .Concat(PileType.Hand.GetPile(Owner).Cards)
            .ToList();

        int selectCount = System.Math.Min(points, allCards.Count);
        if (selectCount == 0)
            return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, selectCount, selectCount);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, allCards, Owner, prefs);
        var toPlay = selected.ToList();
        if (toPlay.Count == 0)
            return;

        foreach (var card in toPlay)
            await CardCmd.AutoPlay(choiceContext, card, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
