using BaseLib.Abstracts;
using BaseLib.Extensions;

namespace Rebellia.RebelliaCode.Api.Powers;

public abstract class RebelliaPowers : CustomPowerModel
{
    public override string CustomPackedIconPath =>
        $"res://{MainFile.ModId}/images/powers/{GetBaseName()}.png";

    public override string CustomBigIconPath =>
        $"res://{MainFile.ModId}/images/powers/{GetBaseName()}.png";

    protected virtual string GetBaseName()
    {
        return Id.Entry.RemovePrefix().ToLowerInvariant();
    }
}