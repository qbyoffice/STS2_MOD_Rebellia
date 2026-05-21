using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Rebellia.RebelliaCode.Api.Extensions;

public static class CardKeywordExtensions
{
    [CustomEnum("REBELLIASANGUINE")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword RebelliaSanguine;

    public static bool IsRebelliaSanguine(this CardModel card)
    {
        return card.Keywords.Contains(RebelliaSanguine);
    }
}