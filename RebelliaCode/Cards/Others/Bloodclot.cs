using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Cards.Common;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Others;

public class Bloodclot() : RebelliaCard(1, CardType.Status, CardRarity.Token, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.BloodclotExhaust];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodclotTrans, HoverTipsValue.CrimsonVeil];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CrimsonVeilPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        int veilAmount = (int)
            DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).BaseValue;
        var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
        veilPower?.AddVeilPoints(veilAmount);

        await CardPileCmd.RemoveFromCombat(this);

        var transformed = combatState.CreateCard<BloodclotTrans>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(
            transformed,
            PileType.Exhaust,
            addedByPlayer: true
        );
    }
}
