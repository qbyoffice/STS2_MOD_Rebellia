using MegaCrit.Sts2.Core.HoverTips;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Cards.Others;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Api;

public static class HoverTipsValue
{
    // 猩红面纱
    public static readonly IHoverTip CrimsonVeil = HoverTipFactory.FromPower<CrimsonVeilPower>();

    // 鲜血剑艺（血点）
    public static readonly IHoverTip BloodSwordArt =
        HoverTipFactory.FromPower<BloodSwordArtPower>();

    // 临时生命
    public static readonly IHoverTip RebelliaTempHp =
        HoverTipFactory.FromPower<RebelliaTmepHpPower>();

    // 猩红打击伤害加成（不可见，但也可添加）
    public static readonly IHoverTip CrimsonStrikeDamage =
        HoverTipFactory.FromPower<CrimsonStrikeDamagePower>();

    // 猩红打击豁免
    public static readonly IHoverTip CrimsonStrike =
        HoverTipFactory.FromPower<CrimsonStrikePower>();

    // 反伤（荆棘）
    public static readonly IHoverTip Rend = HoverTipFactory.FromPower<RendPower>();

    // 血镖免费折扣
    public static readonly IHoverTip BloodDartDiscount =
        HoverTipFactory.FromPower<BloodDartDiscountPower>();

    public static readonly IHoverTip Bloodclot = HoverTipFactory.FromCard<Bloodclot>();

    public static readonly IHoverTip ExtraHoverTips =
        HoverTipFactory.FromPower<CrimsonStrikeDamagePower>();

    public static readonly IHoverTip KeywordSanguine = HoverTipFactory.FromKeyword(
        RCardKeywordExtensions.RebelliaSanguine
    );
}
