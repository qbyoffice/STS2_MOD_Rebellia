using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Uncommon;

public class CrimsonStrike()
    : RebelliaCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        if (Utils.HasAnyPower<CrimsonStrikeDamagePower, CrimsonStrikePower>(Owner.Creature))
            return;

        int extra = Owner.Creature.MaxHp * (IsUpgraded ? 10 : 6) / 100;
        var damagePower = await PowerCmd.Apply<CrimsonStrikeDamagePower>(
            Owner.Creature,
            extra,
            Owner.Creature,
            this
        );
        damagePower?.SetSourceCard(this);

        var freePower = await PowerCmd.Apply<CrimsonStrikePower>(
            Owner.Creature,
            1,
            Owner.Creature,
            this
        );
        freePower?.SetSourceCard(this);
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
