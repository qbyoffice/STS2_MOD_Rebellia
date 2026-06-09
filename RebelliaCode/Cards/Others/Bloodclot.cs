using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Others;

public class Bloodclot() : RebelliaCard(1, CardType.Status, CardRarity.None, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.BloodclotExhaust];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.CrimsonVeil];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CrimsonVeilPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var veilAmount = (int)
            DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).BaseValue;
        var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
        veilPower?.AddVeilPoints(veilAmount);

        AddKeyword(RCardKeywordExtensions.RebelliaSanguine);
    }
}
