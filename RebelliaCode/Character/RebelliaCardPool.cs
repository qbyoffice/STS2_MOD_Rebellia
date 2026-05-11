using BaseLib.Abstracts;
using Godot;

namespace Rebellia.RebelliaCode.Character;

public class RebelliaCardPool : CustomCardPoolModel
{
    public override string Title => Rebellia.InnerName;

    public override string BigEnergyIconPath => "";

    public override string TextEnergyIconPath => "";

    public override string CardFrameMaterialPath => "card_frame_purple";
    public override Color ShaderColor => new("384A61");
    public override float H => 0.75f;
    public override float S => 0.6f;
    public override float V => 1.0f;
    public override Color DeckEntryCardColor => new("4E437F");
    public override Color EnergyOutlineColor => new("4E437F");
    public override bool IsColorless => false;
}
