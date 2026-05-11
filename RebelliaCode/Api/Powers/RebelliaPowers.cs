using BaseLib.Abstracts;
using BaseLib.Extensions;
using Rebellia.RebelliaCode;

namespace Rebellia.RebelliaCode.Api.Powers;

public abstract class RebelliaPowers : CustomPowerModel
{
    protected virtual string GetBaseName() => Id.Entry.RemovePrefix().ToLowerInvariant();

    public override string CustomPackedIconPath =>
        $"res://{MainFile.ModId}/images/powers/{GetBaseName()}.png";

    public override string CustomBigIconPath =>
        $"res://{MainFile.ModId}/images/powers/{GetBaseName()}.png";
}
