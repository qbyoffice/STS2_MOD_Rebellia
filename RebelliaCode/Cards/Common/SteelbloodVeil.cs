using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class SteelBloodVeil() : RebelliaCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<CrimsonVeilPower>(1), new PowerVar<BloodSwordArtPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var veilGain = (int)DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).BaseValue;
        if (veilGain > 0)
        {
            var veilPower = await Utils.GetOrCreatePower<CrimsonVeilPower>(Owner.Creature);
            veilPower?.AddVeilPoints(veilGain);
        }

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
        {
            var currentVeil = Owner.Creature.GetPower<CrimsonVeilPower>()?.GetVeilPoints() ?? 0;
            if (currentVeil > 0)
                await PowerCmd.Apply<PlatingPower>(
                    choiceContext,
                    Owner.Creature,
                    currentVeil,
                    Owner.Creature,
                    this
                );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVarsHelper.GetPowerVar<CrimsonVeilPower>(DynamicVars).UpgradeValueBy(1m);
        EnergyCost.UpgradeBy(-1);
        RemoveKeyword(CardKeyword.Ethereal);
        AddKeyword(RCardKeywordExtensions.RebelliaSanguine);
    }
}
