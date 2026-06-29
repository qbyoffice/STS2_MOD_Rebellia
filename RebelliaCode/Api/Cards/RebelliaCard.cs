using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using Rebellia.RebelliaCode.Character;

namespace Rebellia.RebelliaCode.Api.Cards;

[Pool(typeof(RebelliaCardPool))]
public abstract class RebelliaCard(
    int baseCost,
    CardType type,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true,
    bool autoAdd = true
) : CustomCardModel(baseCost, type, rarity, target, showInCardLibrary, autoAdd)
{
    public override string? CustomPortraitPath =>
        $"res://{MainFile.ModId}/images/card_portraits/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
}
