using BaseLib.Abstracts;
using Godot;

namespace Rebellia.RebelliaCode.Character;

public class RebelliaCardPool : CustomCardPoolModel
{
    public override string Title => Rebellia.InnerName;

    public override string BigEnergyIconPath =>
        "res://Rebellia/images/ui/combat/Rebellia_energy_icon.png";

    public override string TextEnergyIconPath =>
        "res://Rebellia/images/ui/combat/text_Rebellia_energy_icon.png";

    public override string CardFrameMaterialPath => "card_frame_blood";
    public override Color ShaderColor => new("4011");
    public override float H => 0f;
    public override float S => 0.9f;
    public override float V => 0.6f;
    public override Color DeckEntryCardColor => new("74011f");
    public override Color EnergyOutlineColor => new("74011f");
    public override bool IsColorless => false;
}
