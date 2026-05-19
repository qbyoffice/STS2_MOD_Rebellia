using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Cards;
using Rebellia.RebelliaCode.Cards.Others;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Cards.Common
{
    public class SpectralBloodscourge()
        : RebelliaCard(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipsValue.Bloodclot];
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [
                new DamageVar(16m, ValueProp.Move),
                new CardsVar(5),
                new PowerVar<CrimsonVeilPower>(1),
            ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var combatState = Owner.Creature.CombatState;
            if (combatState == null)
                return;

            var cmd = DamageCmd
                .Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(combatState);
            await cmd.Execute(choiceContext);

            for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
            {
                var bloodclot = combatState.CreateCard<Bloodclot>(Owner);
                await CardPileCmd.AddGeneratedCardToCombat(
                    bloodclot,
                    PileType.Draw,
                    addedByPlayer: true,
                    position: CardPilePosition.Random
                );
                await Cmd.Wait(0.05f);
            }

            if (Owner.Creature.GetPower<RebelliaTmepHpPower>() == null)
            {
                await PowerCmd.Apply<RebelliaTmepHpPower>(Owner.Creature, 1, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
        }
    }
}
