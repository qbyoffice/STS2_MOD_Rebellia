using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Api.Powers;

namespace Rebellia.RebelliaCode.Powers.cards;

public class CrimsonStrikePower : RebelliaPowers
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;

    private PowerData Data => GetInternalData<PowerData>();

    public bool IsBloodCostExempted => true;

    protected override object InitInternalData()
    {
        return new PowerData();
    }

    public void SetSourceCard(CardModel source)
    {
        Data.SourceCard = source;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    private class PowerData
    {
        public CardModel? SourceCard;
    }
}
