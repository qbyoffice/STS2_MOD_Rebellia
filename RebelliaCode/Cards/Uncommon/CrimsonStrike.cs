using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class CrimsonStrike()
    : RebelliaCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodSwordArt, HoverTipsValue.ExtraHoverTips];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(9, ValueProp.Move),
            new PowerVar<CrimsonStrikeDamagePower>(6),
            new PowerVar<CrimsonStrikePower>(1),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        if (Utils.HasAnyPower<CrimsonStrikeDamagePower, CrimsonStrikePower>(Owner.Creature))
            return;

        var percent = (int)
            DynamicVarsHelper.GetPowerVar<CrimsonStrikeDamagePower>(DynamicVars).BaseValue;
        var extra = Owner.Creature.MaxHp * percent / 100;
        var damagePower = await PowerCmd.Apply<CrimsonStrikeDamagePower>(
            choiceContext,
            Owner.Creature,
            extra,
            Owner.Creature,
            this
        );
        damagePower?.SetSourceCard(this);

        var freePowerAmount = (int)
            DynamicVarsHelper.GetPowerVar<CrimsonStrikePower>(DynamicVars).BaseValue;
        var freePower = await PowerCmd.Apply<CrimsonStrikePower>(
            choiceContext,
            Owner.Creature,
            freePowerAmount,
            Owner.Creature,
            this
        );
        freePower?.SetSourceCard(this);
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVarsHelper.GetPowerVar<CrimsonStrikeDamagePower>(DynamicVars).UpgradeValueBy(4m);
    }
}
