using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class BloodCrimsonSwift()
    : RebelliaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(9, ValueProp.Move),
            new PowerVar<DexterityPower>(1),
            new PowerVar<BloodCrimsonSwiftPower>(1),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        var dexterityAmount = (int)
            DynamicVarsHelper.GetPowerVar<DexterityPower>(DynamicVars).BaseValue;
        await Utils.GivePower<DexterityPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars,
            Owner.Creature,
            this
        );

        await Utils.GivePower<BloodCrimsonSwiftPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVarsHelper.GetPowerVar<DexterityPower>(DynamicVars).UpgradeValueBy(1m);
        DynamicVarsHelper.GetPowerVar<BloodCrimsonSwiftPower>(DynamicVars).UpgradeValueBy(1m);
    }
}
