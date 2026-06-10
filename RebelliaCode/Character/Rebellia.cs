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
    public override int StartingHp => 70;

    public override CardPoolModel CardPool => ModelDb.CardPool<RebelliaCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<RebelliaRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<RebelliaPotionPool>();

    public override IEnumerable<CardModel> StartingDeck =>
        [
            ModelDb.Card<RebelliaStrike>(),
            ModelDb.Card<RebelliaStrike>(),
            ModelDb.Card<RebelliaStrike>(),
            ModelDb.Card<RebelliaStrike>(),
            ModelDb.Card<RebelliaDefend>(),
            ModelDb.Card<RebelliaDefend>(),
            ModelDb.Card<RebelliaDefend>(),
            ModelDb.Card<RebelliaDefend>(),
            ModelDb.Card<CrimsonPulse>(),
            ModelDb.Card<CrimsonVeil>(),
        ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<LucentCrystal>()];

    public override CustomEnergyCounter? CustomEnergyCounter =>
        new CustomEnergyCounter(
            i => "" + i + ".png",
            new Color(0.6f, 0.1f, 0.1f),
            new Color(0.4f, 0.05f, 0.05f)
        );
}
