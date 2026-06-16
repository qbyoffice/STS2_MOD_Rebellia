using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Cards.Basic;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Ancient;

public class PrimordialCrimsonRaid : RebelliaCard
{
    public PrimordialCrimsonRaid()
        : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(11m, ValueProp.Move), new PowerVar<BloodSwordArtPower>(0)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = play.Target;
        if (target == null)
            return;

        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var bloodPower = Owner.Creature.GetPower<BloodSwordArtPower>();
        if (bloodPower == null)
            return;

        var currentPoints = bloodPower.GetPoints();
        if (currentPoints <= 0)
            return;

        var consumed = await Utils.TryConsumeBloodArtPoints(Owner.Creature, currentPoints);
        if (!consumed)
            return;

        var drawPile = PileType.Draw.GetPile(Owner);
        var availableCards = drawPile.Cards.ToList();
        var selectCount = Math.Min(currentPoints, availableCards.Count);
        if (selectCount == 0)
            return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, selectCount, selectCount);
        var selectedCards = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            availableCards,
            Owner,
            prefs
        );
        var cardsToPlay = selectedCards.ToList();

        if (cardsToPlay.Count == 0)
            return;

        foreach (var card in cardsToPlay)
        {
            if (card is RebelliaStrike)
            {
                var strikeCard = Utils.GetAvailableStrikeCard(Owner);
                if (strikeCard != null)
                    await CardCmd.AutoPlay(choiceContext, strikeCard, target);
            }
            else
            {
                await CardCmd.AutoPlay(choiceContext, card, null);
            }

            await Cmd.Wait(0.1f);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
