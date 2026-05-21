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
    : RebelliaCard(3, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const string TotalHitsKey = "TotalHits";

    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.BloodSwordArt];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(5m, ValueProp.Move),
            new PowerVar<BloodSwordArtPower>(3),
            new CalculationBaseVar(1),
            new CalculationExtraVar(0),
            new CalculatedVar(TotalHitsKey).WithMultiplier(
                (card, target) =>
                {
                    var hand = PileType.Hand.GetPile(card.Owner).Cards;
                    int attackCount = hand.Count(c => c != null && c.Type == CardType.Attack);
                    int baseVal = (int)card.DynamicVars.CalculationBase.BaseValue;
                    int extra = (int)card.DynamicVars.CalculationExtra.BaseValue;
                    return attackCount * (baseVal + extra);
                }
            ),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (!await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            return;

        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var hand = PileType.Hand.GetPile(Owner).Cards;
        int attackCount = hand.Count(c => c.Type == CardType.Attack);
        int baseVal = (int)DynamicVars.CalculationBase.BaseValue;
        int extra = (int)DynamicVars.CalculationExtra.BaseValue;
        int extraAttacks = attackCount * (baseVal + extra);

        for (int i = 0; i < extraAttacks; i++)
        {
            var cmd = DamageCmd
                .Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(play.Target!);
            await cmd.Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
