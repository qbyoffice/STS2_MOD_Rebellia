using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class CrimsonHusk() : RebelliaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.RebelliaTempHp];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CrimsonHuskPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Utils.GivePower<CrimsonHuskPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}