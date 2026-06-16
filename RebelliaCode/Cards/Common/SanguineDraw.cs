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
        [new DamageVar(6m, ValueProp.Move), new PowerVar<BloodSwordArtPower>(1), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var removedVeil = 0;
        var veilPower = Owner.Creature.GetPower<CrimsonVeilPower>();
        if (veilPower != null)
            removedVeil = veilPower.GetVeilPoints();

        Utils.SuppressBloodConsumption(true);
        if (veilPower != null && removedVeil > 0)
            veilPower.AddVeilPoints(-removedVeil);

        Utils.SuppressBloodConsumption(false);

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
        {
            var count = DynamicVars.Cards.IntValue;
            var hand = PileType.Hand.GetPile(Owner).Cards;
            var sanguineCards = hand.Where(c =>
                    c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine)
                )
                .ToList();

            for (var i = 0; i < count && sanguineCards.Count > 0; i++)
            {
                var randomCard = Owner.RunState.Rng.CombatCardSelection.NextItem(sanguineCards);
                if (randomCard != null)
                {
                    await CardCmd.AutoPlay(choiceContext, randomCard, play.Target);
                    sanguineCards = PileType
                        .Hand.GetPile(Owner)
                        .Cards.Where(c =>
                            c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine)
                        )
                        .ToList();
                }
            }
        }

        if (removedVeil > 0)
        {
            var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(
                Owner.Creature,
                0,
                Owner.Creature,
                this,
                choiceContext
            );
            bloodPower!.AddPoints(removedVeil);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}