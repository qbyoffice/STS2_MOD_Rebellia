using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Rebellia.RebelliaCode.Api.Extensions;
using Rebellia.RebelliaCode.Character;

namespace Rebellia.RebelliaCode.Api.Relics;

[Pool(typeof(RebelliaRelicPool))]
public abstract class RebelliaRelics : CustomRelicModel
{
    protected override string BigIconPath => $"{GetBaseFileName()}.png".BigRelicImagePath();
    public override string PackedIconPath => $"{GetBaseFileName()}.tres".TresRelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{GetBaseFileName()}_outline.tres".TresRelicImagePath();

    private string GetBaseFileName()
    {
        return Id.Entry.RemovePrefix().ToLowerInvariant();
    }
}
