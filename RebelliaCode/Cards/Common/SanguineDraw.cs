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
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class SanguineDraw()
    : RebelliaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodSwordArt, HoverTipsValue.CrimsonVeil];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new PowerVar<BloodSwordArtPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var veilPower = Owner.Creature.GetPower<CrimsonVeilPower>();
        if (veilPower != null)
        {
            int current = veilPower.GetVeilPoints();
            if (current > 0)
                veilPower.AddVeilPoints(-current);
        }

        int requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
        {
            var hand = PileType.Hand.GetPile(Owner).Cards;
            var sanguineCards = hand.Where(c =>
                    c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine)
                )
                .ToList();
            if (sanguineCards.Count > 0)
            {
                var randomCard = Owner.RunState.Rng.CombatCardSelection.NextItem(sanguineCards);
                if (randomCard != null)
                {
                    await CardCmd.AutoPlay(
                        choiceContext,
                        randomCard,
                        play.Target,
                        AutoPlayType.Default
                    );
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
