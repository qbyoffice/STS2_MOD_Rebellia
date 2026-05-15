using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Cards.Basic;
using Rebellia.RebelliaCode.Relics;

namespace Rebellia.RebelliaCode.Character;

public class Rebellia : PlaceholderCharacterModel
{
    public const string InnerName = "Rebellia";
    public static readonly Color CharacterColor = new("74011f");

    public override Color NameColor => CharacterColor;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 76;

    public override CardPoolModel CardPool => ModelDb.CardPool<RebelliaCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<RebelliaRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<RebelliaPotionPool>();

    public override IEnumerable<CardModel> StartingDeck =>
        [
            ModelDb.Card<CrimsonPulse>(),
            ModelDb.Card<CrimsonVeil>(),
            ModelDb.Card<RebelliaDefend>(),
            ModelDb.Card<RebelliaDefend>(),
            ModelDb.Card<RebelliaDefend>(),
            ModelDb.Card<RebelliaDefend>(),
            ModelDb.Card<RebelliaStrike>(),
            ModelDb.Card<RebelliaStrike>(),
            ModelDb.Card<RebelliaStrike>(),
            ModelDb.Card<RebelliaStrike>(),
        ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<LucentCrystal>()];

    public override CustomEnergyCounter? CustomEnergyCounter =>
        new CustomEnergyCounter(
            i => "" + i + ".png",
            new Color(0.6f, 0.1f, 0.1f),
            new Color(0.4f, 0.05f, 0.05f)
        );

    public override string CustomAttackSfx => "res://";
    public override string CustomCastSfx => "res://";
    public override string CustomDeathSfx => "res://";
    public override string CustomTrailPath => "";
    public override string CustomVisualPath => "res://Rebellia/scenes/creature_visuals/regent.tscn";
    public override string CustomMerchantAnimPath =>
        "res://Rebellia/scenes/merchant/characters/regent_merchant.tscn";
    public override string CustomRestSiteAnimPath =>
        "res://Rebellia/scenes/rest_site/characters/regent_rest_site.tscn";
    public override string CustomIconPath => "";
    public override string CustomIconTexturePath => "";
    public override string CustomCharacterSelectLockedIconPath => "";
    public override string CustomCharacterSelectIconPath => "";
    public override string CustomArmPointingTexturePath => "";
    public override string CustomArmRockTexturePath => "";
    public override string CustomArmPaperTexturePath => "";
    public override string CustomArmScissorsTexturePath => "";
    public override string CustomCharacterSelectBg =>
        "res://Rebellia/scenes/screens/char_select/char_select_bg_regent.tscn";
    public override string CustomMapMarkerPath => "";
}
