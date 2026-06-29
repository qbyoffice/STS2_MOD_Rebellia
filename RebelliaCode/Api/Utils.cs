using System.Diagnostics;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Rebellia.RebelliaCode.Api.DynamicVars;
using Rebellia.RebelliaCode.Cards.Basic;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Api;

public static class Utils
{
    public static bool IsBloodConsumptionSuppressed { get; private set; }

    // GivePower 重载1：单个目标
    public static async Task GivePower<T>(
        PlayerChoiceContext context,
        Creature target,
        DynamicVarSet varSet,
        Creature? applier,
        CardModel? cardModel
    )
        where T : PowerModel
    {
        await PowerCmd.Apply<T>(
            context,
            target,
            DynamicVarsHelper.GetPowerVar<T>(varSet).BaseValue,
            applier,
            cardModel
        );
    }

    // GivePower 重载2：多个目标
    public static async Task GivePower<T>(
        PlayerChoiceContext context,
        IReadOnlyList<Creature> targets,
        DynamicVarSet varSet,
        Creature? applier,
        CardModel? cardModel
    )
        where T : PowerModel
    {
        await PowerCmd.Apply<T>(
            context,
            targets,
            DynamicVarsHelper.GetPowerVar<T>(varSet).BaseValue,
            applier,
            cardModel
        );
    }

    // GivePower 重载3：从卡牌自动推导目标
    public static async Task GivePower<T>(
        PlayerChoiceContext context,
        CardModel cardModel,
        CardPlay play
    )
        where T : PowerModel
    {
        switch (cardModel.TargetType)
        {
            case TargetType.Self:
                await GivePower<T>(
                    context,
                    cardModel.Owner.Creature,
                    cardModel.DynamicVars,
                    cardModel.Owner.Creature,
                    cardModel
                );
                break;
            case TargetType.AllEnemies:
                Debug.Assert(cardModel.CombatState != null);
                await GivePower<T>(
                    context,
                    cardModel.CombatState.HittableEnemies,
                    cardModel.DynamicVars,
                    cardModel.Owner.Creature,
                    cardModel
                );
                break;
            case TargetType.RandomEnemy:
                Debug.Assert(cardModel.CombatState != null);
                var targets = cardModel.CombatState.HittableEnemies;
                var target = cardModel.Owner.RunState.Rng.CombatTargets.NextItem(targets);
                if (target != null)
                    await GivePower<T>(
                        context,
                        target,
                        cardModel.DynamicVars,
                        cardModel.Owner.Creature,
                        cardModel
                    );
                break;
            default:
                Debug.Assert(play.Target != null);
                await GivePower<T>(
                    context,
                    play.Target,
                    cardModel.DynamicVars,
                    cardModel.Owner.Creature,
                    cardModel
                );
                break;
        }
    }

    // GivePower 重载4：遗物 - 单个目标
    public static async Task GivePower<T>(
        PlayerChoiceContext context,
        RelicModel relicModel,
        Creature target
    )
        where T : PowerModel
    {
        await GivePower<T>(
            context,
            target,
            relicModel.DynamicVars,
            relicModel.Owner.Creature,
            null
        );
    }

    // GivePower 重载5：遗物 - 多个目标
    public static async Task GivePower<T>(
        PlayerChoiceContext context,
        RelicModel relicModel,
        IReadOnlyList<Creature> targets
    )
        where T : PowerModel
    {
        await GivePower<T>(
            context,
            targets,
            relicModel.DynamicVars,
            relicModel.Owner.Creature,
            null
        );
    }

    public static bool IsPoweredAttack(ValueProp props)
    {
        return props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
    }

    public static string GetModelSnakeCase(AbstractModel model)
    {
        return model.Id.Entry.RemovePrefix().ToLowerInvariant();
    }

    public static async Task<IEnumerable<CardModel>> SelectCards(
        Player player,
        LocString selectionPrompt,
        PlayerChoiceContext context,
        PileType pileType,
        int count = 1
    )
    {
        var prefs = new CardSelectorPrefs(selectionPrompt, count);
        var pile = pileType.GetPile(player);
        var cardModelList = pile.Cards;
        if (pile.Type == PileType.Draw)
            cardModelList = cardModelList
                .OrderBy(c => c.Rarity)
                .ThenBy((Func<CardModel, ModelId>)(c => c.Id))
                .ToList();
        return await CardSelectCmd.FromSimpleGrid(context, cardModelList, player, prefs);
    }

    public static async Task<IEnumerable<CardModel>> SelectCards(
        Player player,
        LocString selectionPrompt,
        PlayerChoiceContext context,
        PileType pileType,
        int minCount,
        int maxCount
    )
    {
        var prefs = new CardSelectorPrefs(selectionPrompt, minCount, maxCount);
        var pile = pileType.GetPile(player);
        var cardModelList = pile.Cards;
        if (pile.Type == PileType.Draw)
            cardModelList = cardModelList
                .OrderBy(c => c.Rarity)
                .ThenBy((Func<CardModel, ModelId>)(c => c.Id))
                .ToList();
        return await CardSelectCmd.FromSimpleGrid(context, cardModelList, player, prefs);
    }

    public static async Task<CardModel?> SelectSingleCard(
        Player player,
        LocString selectionPrompt,
        PlayerChoiceContext context,
        PileType pileType
    )
    {
        var prefs = new CardSelectorPrefs(selectionPrompt, 1);
        var pile = pileType.GetPile(player);
        var cardModelList = pile.Cards;
        if (pile.Type == PileType.Draw)
            cardModelList = cardModelList
                .OrderBy(c => c.Rarity)
                .ThenBy((Func<CardModel, ModelId>)(c => c.Id))
                .ToList();
        return (
            await CardSelectCmd.FromSimpleGrid(context, cardModelList, player, prefs)
        ).FirstOrDefault();
    }

    public static async Task<T?> GetOrCreatePower<T>(
        Creature target,
        decimal initialAmount = 1,
        Creature? applier = null,
        CardModel? cardSource = null,
        PlayerChoiceContext? context = null
    )
        where T : PowerModel
    {
        var power = target.GetPower<T>();
        if (power != null)
            return power;

        power = await PowerCmd.Apply<T>(
            context,
            target,
            initialAmount,
            applier ?? target,
            cardSource
        );
        return power;
    }

    public static async Task<T?> ApplyPower<T>(
        Creature target,
        decimal amount,
        Creature? applier = null,
        CardModel? cardSource = null,
        bool silent = false,
        PlayerChoiceContext? context = null
    )
        where T : PowerModel
    {
        return await PowerCmd.Apply<T>(
            context,
            target,
            amount,
            applier ?? target,
            cardSource,
            silent
        );
    }

    public static bool HasAnyPower<T1, T2>(Creature creature)
        where T1 : PowerModel
        where T2 : PowerModel
    {
        return creature.GetPower<T1>() != null || creature.GetPower<T2>() != null;
    }

    public static CardModel? GetAvailableStrikeCard(Player player)
    {
        var combatState = player?.PlayerCombatState;
        if (combatState == null)
            return null;

        var handCard = combatState.Hand?.Cards?.FirstOrDefault(c => c is RebelliaStrike);
        if (handCard != null)
            return handCard;

        return combatState.DrawPile?.Cards?.FirstOrDefault(c => c is RebelliaStrike);
    }

    public static bool IsBloodCostExempted(Creature creature)
    {
        return creature.GetPower<CrimsonStrikePower>() != null;
    }

    public static event Func<Creature, Task>? BloodArtConsumed;

    public static async Task<bool> TryConsumeBloodArtPoints(Creature creature, int requiredPoints)
    {
        if (IsBloodConsumptionSuppressed)
        {
            if (BloodArtConsumed != null)
                await BloodArtConsumed.Invoke(creature);
            return true;
        }

        var exemptPower = creature.GetPower<CrimsonStrikePower>();
        if (exemptPower != null)
        {
            var damagePower = creature.GetPower<CrimsonStrikeDamagePower>();
            if (damagePower != null)
                await PowerCmd.Remove(damagePower);
            await PowerCmd.Remove(exemptPower);
            if (BloodArtConsumed != null)
                await BloodArtConsumed.Invoke(creature);
            return true;
        }

        var bloodPower = await GetOrCreatePower<BloodSwordArtPower>(creature);
        if (bloodPower == null || bloodPower.GetPoints() < requiredPoints)
            return false;
        bool success = bloodPower.TrySpendPoints(requiredPoints);
        if (success && BloodArtConsumed != null)
            await BloodArtConsumed.Invoke(creature);
        return success;
    }

    public static async Task<bool> TryUpgradeToAllEnemies(
        CardModel card,
        PlayerChoiceContext context,
        CardPlay play,
        int requiredBlood,
        decimal damage,
        Func<Task>? onConsumeSuccess = null
    )
    {
        var combatState = card.Owner.Creature.CombatState;
        if (combatState == null)
            return false;

        if (await TryConsumeBloodArtPoints(card.Owner.Creature, requiredBlood))
        {
            var baseCmd = DamageCmd.Attack(damage).FromCard(card);
            foreach (var enemy in combatState.HittableEnemies)
                await baseCmd.Targeting(enemy).Execute(context);
            if (onConsumeSuccess != null)
                await onConsumeSuccess();
            return true;
        }

        return false;
    }

    public static void SuppressBloodConsumption(bool suppress)
    {
        IsBloodConsumptionSuppressed = suppress;
    }
}
