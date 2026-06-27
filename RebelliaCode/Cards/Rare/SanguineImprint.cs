using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class SanguineImprint() : RebelliaCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const string CloneCountKey = "CloneCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CardsVar(1),
            new CalculationBaseVar(0),
            new CalculationExtraVar(1),
            new CalculatedVar(CloneCountKey).WithMultiplier(
                (card, target) =>
                {
                    var bloodPower = card.Owner.Creature.GetPower<BloodSwordArtPower>();
                    int points = bloodPower?.GetPoints() ?? 0;
                    return points;
                }
            ),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var handPile = PileType.Hand.GetPile(Owner);
        var attackCards = handPile.Cards.Where(c => c.Type == CardType.Attack).ToList();

        if (attackCards.Count == 0)
            return;

        int currentBloodPoints = Owner.Creature.GetPower<BloodSwordArtPower>()?.GetPoints() ?? 0;
        if (currentBloodPoints <= 0)
            return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, (int)DynamicVars.Cards.BaseValue);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, attackCards, Owner, prefs);
        var originalCard = selected.FirstOrDefault();
        if (originalCard == null)
            return;

        originalCard.SetToFreeThisCombat();
        originalCard.AddKeyword(CardKeyword.Exhaust);

        if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, currentBloodPoints))
        {
            bool upgradeClones = IsUpgraded;
            for (int i = 0; i < currentBloodPoints; i++)
            {
                var clone = originalCard.CreateClone();

                if (upgradeClones && !clone.IsUpgraded)
                {
                    CardCmd.Upgrade(clone);
                }

                var addResult = await CardPileCmd.AddGeneratedCardToCombat(
                    clone,
                    PileType.Hand,
                    Owner
                );
                CardCmd.PreviewCardPileAdd(addResult);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
