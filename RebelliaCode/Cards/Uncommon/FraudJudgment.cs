using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Cards.Others;

namespace Rebellia.RebelliaCode.Cards.Common;

public class FraudJudgment() : RebelliaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.SwiftBloodWeapon];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        int count = (int)DynamicVars.Cards.BaseValue;
        var generatedCards = new List<CardModel>();

        for (int i = 0; i < count; i++)
        {
            var card = combatState.CreateCard<SwiftBloodWeapon>(Owner);
            if (IsUpgraded)
                CardCmd.Upgrade(card);
            await CardPileCmd.AddGeneratedCardToCombat(
                card,
                PileType.Draw,
                Owner,
                CardPilePosition.Top
            );
            generatedCards.Add(card);
        }

        if (generatedCards.Count > 0)
            CardCmd.Preview(generatedCards, time: 1.0f, CardPreviewStyle.HorizontalLayout);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
