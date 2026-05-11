using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace LittleWizard.LittleWizardCode.Api.Animation;

[GlobalClass]
public partial class SNVfxSpine : NVfxSpine
{
    public SNVfxSpine()
    {
        var field = typeof(NVfxSpine).GetField(
            "_animation",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        if (field != null)
            field.SetValue(this, "animation");
    }
}
