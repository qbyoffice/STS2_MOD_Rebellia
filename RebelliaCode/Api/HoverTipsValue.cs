using MegaCrit.Sts2.Core.HoverTips;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Cards.Basic;
using Rebellia.RebelliaCode.Cards.Others;
using Rebellia.RebelliaCode.Powers;
using Rebellia.RebelliaCode.Powers.cards;
using Rebellia.RebelliaCode.Powers.tools;

namespace Rebellia.RebelliaCode.Api;

public static class HoverTipsValue
{
    public static readonly IHoverTip CrimsonVeil = HoverTipFactory.FromPower<CrimsonVeilPower>();

    public static readonly IHoverTip BloodSwordArt =
        HoverTipFactory.FromPower<BloodSwordArtPower>();

    public static readonly IHoverTip RebelliaTempHp =
        HoverTipFactory.FromPower<RebelliaTmepHpPower>();

    public static readonly IHoverTip CrimsonStrikeDamage =
        HoverTipFactory.FromPower<CrimsonStrikeDamagePower>();

    public static readonly IHoverTip CrimsonStrike =
        HoverTipFactory.FromPower<CrimsonStrikePower>();

    public static readonly IHoverTip Rend = HoverTipFactory.FromPower<RendPower>();

    public static readonly IHoverTip BloodDartDiscount =
        HoverTipFactory.FromPower<BloodDartDiscountPower>();

    public static readonly IHoverTip Bloodclot = HoverTipFactory.FromCard<Bloodclot>();

    public static readonly IHoverTip Bloodshiv = HoverTipFactory.FromCard<DartBloodWeapon>();

    public static readonly IHoverTip RebelliaStrike = HoverTipFactory.FromCard<RebelliaStrike>();

    public static readonly IHoverTip SwiftBloodWeapon =
        HoverTipFactory.FromCard<SwiftBloodWeapon>();

    public static readonly IHoverTip ExtraHoverTips =
        HoverTipFactory.FromPower<CrimsonStrikeDamagePower>();

    public static readonly IHoverTip KeywordSanguine = HoverTipFactory.FromKeyword(
        RCardKeywordExtensions.RebelliaSanguine
    );

    public static readonly IHoverTip ErodingBlood = HoverTipFactory.FromPower<ErodingBloodPower>();

    public static readonly IHoverTip BloodPierce = HoverTipFactory.FromPower<BloodpierceTool>();

    public static readonly IHoverTip SanguineExtract =
        HoverTipFactory.FromPower<SanguineExtractTool>();

    public static readonly IHoverTip BloodWeaponVaultTool =
        HoverTipFactory.FromPower<BloodWeaponVaultTool>();

    public static readonly IHoverTip ArmorPower = HoverTipFactory.FromPower<ArmorPower>();

    public static readonly IHoverTip CrimsonVeilTool = HoverTipFactory.FromPower<CrimsonVeilTool>();
}