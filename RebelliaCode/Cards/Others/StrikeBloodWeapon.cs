using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Cards.Others;

public class StrikeBloodWeapon()
    : RebelliaCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags =>
        [
            CardTag.Strike,
            CardTagExtensions.RebelliaBloodWeapon,
            CardTagExtensions.RebelliaBloodWeaponArt,
        ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new PowerVar<BloodSwordArtPower>(1), new CardsVar(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var hand = PileType.Hand.GetPile(Owner).Cards;
        var strikeCards = hand.Where(c => c != this && c.Tags.Contains(CardTag.Strike)).ToList();
        if (strikeCards.Count == 0)
            return;

        int requiredBlood = (int)
            DynamicVarsHelper.GetPowerVar<BloodSwordArtPower>(DynamicVars).BaseValue;
        if (await Utils.TryConsumeBloodArtPoints(Owner.Creature, requiredBlood))
        {
            int count = DynamicVars.Cards.IntValue;
            for (int i = 0; i < count && strikeCards.Count > 0; i++)
            {
                var randomStrike = Owner.RunState.Rng.CombatCardSelection.NextItem(strikeCards);
                if (randomStrike != null)
                {
                    await CardCmd.AutoPlay(
                        choiceContext,
                        randomStrike,
                        play.Target,
                        AutoPlayType.Default
                    );
                    strikeCards = PileType
                        .Hand.GetPile(Owner)
                        .Cards.Where(c => c != this && c.Tags.Contains(CardTag.Strike))
                        .ToList();
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
