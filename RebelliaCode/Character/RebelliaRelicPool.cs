using BaseLib.Abstracts;
using Godot;

namespace Rebellia.RebelliaCode.Character;

public class RebelliaRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Rebellia.InnerName;

    public override Color LabOutlineColor => Rebellia.CharacterColor;
}