using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class BloodForgedBulwark()
    : RebelliaCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new HpLossVar(1m), new BlockVar(6m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            this
        );

        var baseBlock = (int)DynamicVars.Block.BaseValue;
        var tempHpPower = Owner.Creature.GetPower<RebelliaTmepHpPower>();
        var hasTempHp = tempHpPower != null && tempHpPower.GetCurrentTempHp() > 0;
        var finalBlock = hasTempHp ? baseBlock * 2 : baseBlock;

        var blockVar = new BlockVar(finalBlock, ValueProp.Move);
        await CommonActions.CardBlock(this, blockVar, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
