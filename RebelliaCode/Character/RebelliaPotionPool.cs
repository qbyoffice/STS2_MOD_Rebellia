using BaseLib.Abstracts;
using Godot;

namespace Rebellia.RebelliaCode.Character;

public class RebelliaPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => Rebellia.InnerName;

    public override Color LabOutlineColor => Rebellia.CharacterColor;
}