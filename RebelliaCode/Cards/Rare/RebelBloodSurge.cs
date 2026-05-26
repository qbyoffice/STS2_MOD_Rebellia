using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Rare;

public class RebelBloodSurge() : RebelliaCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.CrimsonVeil, HoverTipsValue.BloodSwordArt];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<CrimsonVeilPower>(2),
            new PowerVar<BloodSwordArtPower>(4),
            new PowerVar<ArmorPower>(3),
            new CardsVar(3),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var drawCount = DynamicVars.Cards.IntValue;
        if (drawCount > 0)
            await CardPileCmd.Draw(choiceContext, drawCount, Owner);

        var veilAmount = (int)
            DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).BaseValue;
        if (veilAmount > 0)
        {
            var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
            veilPower?.AddVeilPoints(veilAmount);
        }

        var bloodAmount = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (bloodAmount > 0)
            await BloodSwordArtManager.AddPoints(Owner.Creature, bloodAmount);

        var armorAmount = (int)DynamicVarsHelper.GetPowerVar<ArmorPower>(DynamicVars).BaseValue;
        await Utils.GivePower<ArmorPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2m);
        DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).UpgradeValueBy(1m);
        DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).UpgradeValueBy(2m);
        DynamicVarsHelper.GetPowerVar<ArmorPower>(DynamicVars).UpgradeValueBy(1m);
    }
}
