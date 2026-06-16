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
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Common;

public class VillageGuard()
    : RebelliaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const string ExtraHitsKey = "TotalHits";

    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.BloodSwordArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(10m, ValueProp.Move),
            new PowerVar<BloodSwordArtPower>(2),
            new CalculationBaseVar(1),
            new CalculationExtraVar(1),
            new CalculatedVar(ExtraHitsKey).WithMultiplier(
                (card, target) =>
                {
                    var hand = PileType.Hand.GetPile(card.Owner).Cards;
                    var attackCount = hand.Count(c => c.Type == CardType.Attack);
                    var baseVal = card.DynamicVars.CalculationBase.BaseValue;
                    return attackCount * baseVal - 1;
                }
            ),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (!await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            return;

        var extraHitsVar = DynamicVars[ExtraHitsKey] as CalculatedVar;
        var extraHits = (int)(extraHitsVar?.Calculate(play.Target) ?? 0m);
        if (extraHits <= 0)
            return;

        var damage = DynamicVars.Damage.BaseValue;
        for (var i = 0; i < extraHits; i++)
        {
            var cmd = DamageCmd.Attack(damage).FromCard(this).Targeting(play.Target!);
            await cmd.Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars.CalculationBase.UpgradeValueBy(1);
    }
}
