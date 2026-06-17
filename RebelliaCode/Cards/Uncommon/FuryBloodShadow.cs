using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class FuryBloodShadow()
    : RebelliaCard(3, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<BloodShadow>(), HoverTipsValue.KeywordSanguine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<BloodShadow>(5),
            new PowerVar<FuryBloodShadowPower>(1),
            new PowerVar<FuryBloodShadowUpgradedPower>(1),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // await FuryBloodShadowManager.ApplyFuryCard(choiceContext, Owner.Creature, this, IsUpgraded);

        await PowerCmd.Apply<BloodShadow>(
            choiceContext,
            Owner.Creature,
            DynamicVarsHelper.GetPowerVar<BloodShadow>(DynamicVars).BaseValue,
            Owner.Creature,
            this
        );

        if (IsUpgraded)
        {
            await Utils.GivePower<FuryBloodShadowUpgradedPower>(choiceContext, this, play);
        }
        else
        {
            await Utils.GivePower<FuryBloodShadowPower>(choiceContext, this, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVarsHelper.GetPowerVar<BloodShadow>(DynamicVars).UpgradeValueBy(2);
    }
}
