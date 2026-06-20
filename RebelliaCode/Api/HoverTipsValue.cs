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
    public static IHoverTip CrimsonVeil => HoverTipFactory.FromPower<CrimsonVeilPower>();

    public static IHoverTip BloodSwordArt => HoverTipFactory.FromPower<BloodSwordArtPower>();

    public static IHoverTip RebelliaTempHp => HoverTipFactory.FromPower<RebelliaTmepHpPower>();

    public static IHoverTip CrimsonStrikeDamage =>
        HoverTipFactory.FromPower<CrimsonStrikeDamagePower>();

    public static IHoverTip CrimsonStrike => HoverTipFactory.FromPower<CrimsonStrikePower>();

    public static IHoverTip Rend => HoverTipFactory.FromPower<RendPower>();

    public static IHoverTip BloodDartDiscount =>
        HoverTipFactory.FromPower<BloodDartDiscountPower>();

    public static IHoverTip Bloodclot => HoverTipFactory.FromCard<Bloodclot>();

    public static IHoverTip Bloodshiv => HoverTipFactory.FromCard<DartBloodWeapon>();

    public static IHoverTip RebelliaStrike => HoverTipFactory.FromCard<RebelliaStrike>();

    public static IHoverTip SwiftBloodWeapon => HoverTipFactory.FromCard<SwiftBloodWeapon>();

    public static IHoverTip ExtraHoverTips => HoverTipFactory.FromPower<CrimsonStrikeDamagePower>();

    public static IHoverTip KeywordSanguine =>
        HoverTipFactory.FromKeyword(RCardKeywordExtensions.RebelliaSanguine);

    public static IHoverTip ErodingBlood => HoverTipFactory.FromPower<ErodingBloodPower>();

    public static IHoverTip BloodPierce => HoverTipFactory.FromPower<BloodpierceTool>();

    public static IHoverTip SanguineExtract => HoverTipFactory.FromPower<SanguineExtractTool>();

    public static IHoverTip BloodWeaponVaultTool =>
        HoverTipFactory.FromPower<BloodWeaponVaultTool>();

    public static IHoverTip ArmorPower => HoverTipFactory.FromPower<ArmorPower>();

    public static IHoverTip CrimsonVeilTool => HoverTipFactory.FromPower<CrimsonVeilTool>();
}
