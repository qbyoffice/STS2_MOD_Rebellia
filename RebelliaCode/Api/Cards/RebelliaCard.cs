using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
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
    private const string DefaultCardImage = "a_rebellia_beat";

    public override string? CustomPortraitPath
    {
        get
        {
            var cardId = Id.Entry.RemovePrefix().ToLowerInvariant();
            var targetPath = $"res://{MainFile.ModId}/images/card_portraits/{cardId}.png";

            return ResourceLoader.Exists(targetPath)
                ? targetPath
                : $"res://{MainFile.ModId}/images/card_portraits/{DefaultCardImage}.png";
        }
    }
}
