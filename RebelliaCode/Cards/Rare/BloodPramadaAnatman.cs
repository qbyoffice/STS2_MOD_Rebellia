using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class BloodPramadaAnatman()
    : RebelliaCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var currentHp = Owner.Creature.CurrentHp;
        var tempPower = await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner.Creature);
        tempPower?.AddTempHp(currentHp);

        await CreatureCmd.SetCurrentHp(Owner.Creature, 1m);

        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
            if (!card.EnergyCost.CostsX)
                card.SetToFreeThisTurn();
    }
}
