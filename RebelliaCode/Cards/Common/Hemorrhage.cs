using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.Extensions;

namespace Rebellia.RebelliaCode.Cards.Common;

public class Hemorrhage() : RebelliaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.Bloodclot];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new HpLossVar(3m), new CardsVar(3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            this
        );

        var drawPile = PileType.Draw.GetPile(Owner);
        var cardsInDraw = drawPile.Cards.ToList();
        if (cardsInDraw.Count == 0)
            return;

        var count = (int)DynamicVars.Cards.BaseValue;
        var rng = Owner.RunState.Rng.CombatCardSelection;
        var shuffled = cardsInDraw.OrderBy(_ => rng.NextInt()).ToList();
        var toModify = shuffled.Take(count).ToList();

        var modifiedCards = new List<CardModel>();

        foreach (var card in toModify)
        {
            if (!card.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
            {
                card.AddKeyword(RCardKeywordExtensions.RebelliaSanguine);
                modifiedCards.Add(card);
            }
        }

        if (modifiedCards.Count > 0)
        {
            CardCmd.Preview(modifiedCards, time: 1.0f, CardPreviewStyle.HorizontalLayout);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
