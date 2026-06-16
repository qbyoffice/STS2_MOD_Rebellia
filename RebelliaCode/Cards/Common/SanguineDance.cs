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

public class SanguineDance()
    : RebelliaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodSwordArt, HoverTipsValue.CrimsonVeil, HoverTipsValue.CrimsonVeilTool];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<CrimsonVeilPower>(1),
        new PowerVar<BloodSwordArtPower>(1),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var veilGain = (int)DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).BaseValue;
        if (!Owner.Creature.HasPower<CrimsonVeilPower>())
        {
            var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
            veilPower?.AddVeilPoints(veilGain);
        }

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (!await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            return;

        var drawPile = PileType.Draw.GetPile(Owner);
        var discardPile = PileType.Discard.GetPile(Owner);
        var attackCards = drawPile
            .Cards.Concat(discardPile.Cards)
            .Where(c => c.Type == CardType.Attack)
            .ToList();

        if (attackCards.Count == 0)
            return;

        var randomCard = Owner.RunState.Rng.CombatCardSelection.NextItem(attackCards);
        if (randomCard == null)
            return;
        var addResult = await CardPileCmd.Add(randomCard, PileType.Hand);
        CardCmd.PreviewCardPileAdd(addResult);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}