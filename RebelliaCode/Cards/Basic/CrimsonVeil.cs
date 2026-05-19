using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Basic;

public class CrimsonVeil() : RebelliaCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Defend, CardTagExtensions.RebelliaBloodWeaponArt];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.CrimsonVeil];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(5, ValueProp.Move), new PowerVar<CrimsonVeilPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        int veilGain = (int)DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).BaseValue;
        if (veilGain > 0)
        {
            var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
            veilPower?.AddVeilPoints(veilGain);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(3m);
        DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).UpgradeValueBy(1m);
    }
}
