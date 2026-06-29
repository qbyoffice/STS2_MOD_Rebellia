using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class BlackStreetShadowPower : RebelliaPowers
{
    private const int BonusAgility = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;
    public override int DisplayAmount => BonusAgility;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override object InitInternalData() => new Data();

    private class Data
    {
        public int Deducted { get; set; }
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await PowerCmd.Apply<DexterityPower>(
            new BlockingPlayerChoiceContext(),
            Owner,
            BonusAgility,
            Owner,
            null
        );
        await base.AfterApplied(applier, cardSource);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Side)
            return;

        var data = GetInternalData<Data>();
        if (data.Deducted == 1)
        {
            var dexterity = Owner.GetPower<DexterityPower>();
            if (dexterity != null)
            {
                await PowerCmd.ModifyAmount(
                    new BlockingPlayerChoiceContext(),
                    dexterity,
                    1,
                    null,
                    null
                );
            }
            data.Deducted = 0;
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;
        if (cardPlay.Card.Type != CardType.Attack)
            return;

        var data = GetInternalData<Data>();
        if (data.Deducted == 0)
        {
            var dexterity = Owner.GetPower<DexterityPower>();
            if (dexterity != null)
            {
                await PowerCmd.ModifyAmount(choiceContext, dexterity, -1, null, null);
            }
            data.Deducted = 1;
        }
    }
}
