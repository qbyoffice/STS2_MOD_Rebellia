using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

class BodyMindCurseUpgradedPower : RebelliaPowers
{
    protected virtual int PowerPerCurse => 2;
    protected virtual int EnergyOnExhaust => 3;
    protected virtual int DrawOnExhaust => 3;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private int GetCurseCount()
    {
        if (Owner?.Player == null)
            return 0;
        var player = Owner.Player;
        var allCards = PileType
            .Draw.GetPile(player)
            .Cards.Concat(PileType.Hand.GetPile(player).Cards)
            .Concat(PileType.Discard.GetPile(player).Cards);
        return allCards.Count(c => c.Type == CardType.Curse);
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        int curseCount = GetCurseCount();
        int strengthGain = curseCount * PowerPerCurse;
        if (strengthGain > 0)
        {
            await PowerCmd.Apply<StrengthPower>(
                new BlockingPlayerChoiceContext(),
                Owner,
                strengthGain,
                Owner,
                null
            );
        }
        await base.AfterApplied(applier, cardSource);
    }

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal
    )
    {
        if (card.Owner != Owner.Player)
            return;
        if (card.Type != CardType.Curse)
            return;

        await PlayerCmd.GainEnergy(EnergyOnExhaust, Owner.Player);
        await CardPileCmd.Draw(choiceContext, DrawOnExhaust, Owner.Player);
    }
}
