using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class BloodclotTrans() : RebelliaCard(1, CardType.Status, CardRarity.Token, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.BloodclotExhaust];
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust, CardKeywordExtensions.RebelliaSanguine];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.CrimsonVeil];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CrimsonVeilPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int veilAmount = (int)
            DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).BaseValue;
        var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
        veilPower?.AddVeilPoints(veilAmount);
    }

    protected override void OnUpgrade() { }
}
