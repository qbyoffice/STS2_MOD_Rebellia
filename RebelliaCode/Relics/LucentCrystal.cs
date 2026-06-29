using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using Rebellia.RebelliaCode.Api;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Api.Powers;
using Rebellia.RebelliaCode.Api.Relics;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Relics;

public class LucentCrystal : RebelliaRelics
{
    private const int MaxCounter = 10;
    private const int FirstUpgradeThreshold = 5;
    private const int SecondUpgradeThreshold = 10;
    private int _bloodBlessing;

    private int _bloodEssence;

    [SavedProperty]
    public int BloodEssence
    {
        get => _bloodEssence;
        set
        {
            if (_bloodEssence != value)
            {
                _bloodEssence = value;
                UpdateDynamicVars();
                InvokeDisplayAmountChanged();
            }
        }
    }

    [SavedProperty]
    public int BloodBlessing
    {
        get => _bloodBlessing;
        set
        {
            if (_bloodBlessing != value)
            {
                _bloodBlessing = value;
                UpdateDynamicVars();
                InvokeDisplayAmountChanged();
            }
        }
    }

    [SavedProperty]
    public bool HasSwitchedOnce { get; set; }

    [SavedProperty]
    public bool HasSwitchedTwice { get; set; }

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    public override int DisplayAmount => BloodEssence + BloodBlessing;

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DynamicVar("BloodEssence", BloodEssence);
            yield return new DynamicVar("BloodBlessing", BloodBlessing);
            yield return new DynamicVar("MaxCounter", MaxCounter);
        }
    }

    private bool CanSwitchFirst()
    {
        return !HasSwitchedOnce
            && (BloodEssence >= FirstUpgradeThreshold || BloodBlessing >= FirstUpgradeThreshold);
    }

    private bool CanSwitchSecond()
    {
        return HasSwitchedOnce
            && !HasSwitchedTwice
            && (BloodEssence >= SecondUpgradeThreshold || BloodBlessing >= SecondUpgradeThreshold);
    }

    private void UpdateDynamicVars()
    {
        if (DynamicVars != null)
        {
            DynamicVars["BloodEssence"].BaseValue = BloodEssence;
            DynamicVars["BloodBlessing"].BaseValue = BloodBlessing;
        }
    }

    public override async Task BeforeCombatStart()
    {
        var bloodPower = await Utils.GetOrCreatePower<BloodSwordArtPower>(Owner.Creature);
        if (bloodPower == null)
            return;

        if (bloodPower.BloodArtMaxPoints < 2)
            bloodPower.BloodArtMaxPoints = 2;

        if (bloodPower.GetPoints() < 2)
            bloodPower.AddPoints(1);
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (room.RoomType == RoomType.Monster)
        {
            if (BloodEssence < MaxCounter)
            {
                BloodEssence++;
                Flash();
            }
        }
        else if (room.RoomType == RoomType.Elite || room.RoomType == RoomType.Event)
        {
            if (BloodBlessing < MaxCounter)
            {
                BloodBlessing++;
                Flash();
            }
        }

        await Task.CompletedTask;
    }

    public async Task TrySwitchToBloodCrystal()
    {
        if (CanSwitchFirst())
        {
            await RelicCmd.Replace(this, ModelDb.Relic<BloodCrystal>().ToMutable());
            HasSwitchedOnce = true;
        }
        else if (CanSwitchSecond())
        {
            await RelicCmd.Replace(this, ModelDb.Relic<BloodCrystal>().ToMutable());
            HasSwitchedTwice = true;
        }
    }

    public async Task TrySwitchToGlimmerCrystal()
    {
        if (CanSwitchFirst())
        {
            await RelicCmd.Replace(this, ModelDb.Relic<GlimmerCrystal>().ToMutable());
            HasSwitchedOnce = true;
        }
        else if (CanSwitchSecond())
        {
            await RelicCmd.Replace(this, ModelDb.Relic<GlimmerCrystal>().ToMutable());
            HasSwitchedTwice = true;
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner == null)
            return;

        if (Owner.Creature.HasPower<CrimsonVeilPower>())
            await BloodKeywordManager.MoveBloodCardsToDrawPile(Owner);
        else
            await BloodKeywordManager.ConsumeAllBloodCards(Owner);
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext ctx,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState state
    )
    {
        if (side != Owner.Creature.Side || state.RoundNumber != 1)
            return;

        await CrimsonVeilPowerManager.TryPlayOrExhaustStatusCard(Owner);

        if (Owner.Creature.HasPower<CrimsonVeilPower>())
            await BloodKeywordManager.MoveBloodCardsToDrawPile(Owner);
        else
            await BloodKeywordManager.ConsumeAllBloodCards(Owner);
    }

    public override async Task AfterFlush(
        PlayerChoiceContext choiceContext,
        Player player,
        IReadOnlyCollection<CardModel> flushedCards,
        IReadOnlyCollection<CardModel> retainedCards
    )
    {
        if (player != Owner)
            return;

        if (!Owner.Creature.HasPower<CrimsonVeilPower>())
        {
            var bloodCards = flushedCards
                .Where(c => c.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine))
                .ToList();
            if (bloodCards.Count > 0)
                foreach (var card in bloodCards)
                    if (card.Pile?.Type != PileType.Exhaust)
                        await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), card);
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        InvokeDisplayAmountChanged();
    }
}
