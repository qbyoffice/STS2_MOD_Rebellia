using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Cards.Others;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common;

public class SpectralBloodScourge()
    : RebelliaCard(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.Bloodclot];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(20m, ValueProp.Move),
            new CardsVar(5),
            new PowerVar<CrimsonVeilPower>(1),
            new PowerVar<RebelliaTmepHpPower>(1),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var cmd = DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingAllOpponents(combatState);
        await cmd.Execute(choiceContext);

        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var bloodclot = combatState.CreateCard<Bloodclot>(Owner);
            var addResult = await CardPileCmd.AddGeneratedCardToCombat(
                bloodclot,
                PileType.Draw,
                Owner,
                CardPilePosition.Random
            );
            CardCmd.PreviewCardPileAdd(addResult);
            await Cmd.Wait(0.05f);
        }

        var tempHpGain = (int)
            DynamicVarsHelper.GetPowerVar<RebelliaTmepHpPower>(DynamicVars).BaseValue;
        if (tempHpGain > 0)
            await Utils.GetOrCreatePower<RebelliaTmepHpPower>(Owner.Creature);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
