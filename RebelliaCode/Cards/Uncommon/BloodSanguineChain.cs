using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class BloodSanguineChain()
    : RebelliaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(9m, ValueProp.Move),
            new PowerVar<ArmorPower>(1),
            new PowerVar<BloodSanguineChainPower>(1),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        if (Owner.Creature.HasPower<CrimsonVeilPower>())
        {
            await PowerCmd.Apply<ArmorPower>(
                choiceContext,
                Owner.Creature,
                DynamicVarsHelper.GetPowerVar<ArmorPower>(DynamicVars).BaseValue,
                Owner.Creature,
                this
            );
        }

        var power = await PowerCmd.Apply<BloodSanguineChainPower>(
            choiceContext,
            Owner.Creature,
            DynamicVarsHelper.GetPowerVar<BloodSanguineChainPower>(DynamicVars).BaseValue,
            Owner.Creature,
            this
        );
        if (power is BloodSanguineChainPower chainPower)
            chainPower.SetLinkedCard(this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVarsHelper.GetPowerVar<ArmorPower>(DynamicVars).UpgradeValueBy(1m);
    }
}
