using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Cards.Ancient;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Basic;

public class CrimsonPulse()
    : RebelliaCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy),
        ITranscendenceCard
{
    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<PrimordialCrimsonRaid>();
    }

    protected override HashSet<CardTag> CanonicalTags => [CardTagExtensions.RebelliaBloodWeaponArt];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7, ValueProp.Move), new PowerVar<BloodSwordArtPower>(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipsValue.BloodSwordArt, HoverTipsValue.RebelliaStrike];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var handStrike = PileType
            .Hand.GetPile(Owner)
            .Cards.FirstOrDefault(c => c is RebelliaStrike);
        var drawStrike = PileType
            .Draw.GetPile(Owner)
            .Cards.FirstOrDefault(c => c is RebelliaStrike);

        var requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;

        if (handStrike == null && drawStrike == null)
        {
            await BloodSwordArtManager.AddPoints(Owner.Creature, 1, choiceContext);
            return;
        }

        if (handStrike == null && drawStrike != null)
        {
            if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            {
                await BloodSwordArtManager.AddPoints(Owner.Creature, 1, choiceContext);
                await CardCmd.AutoPlay(choiceContext, drawStrike, play.Target);
            }
            return;
        }

        if (handStrike != null)
        {
            if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
            {
                await CardCmd.AutoPlay(choiceContext, handStrike, play.Target);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
