using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Rebellia.RebelliaCode.Api.DynamicVars;

public class PlayCountVar : IntVar
{
    public const string DefaultName = "PlayCount";

    public PlayCountVar(decimal baseValue)
        : base(DefaultName, baseValue)
    {
    }
}

public class ExtraAttackCountVar : IntVar
{
    public const string DefaultName = "ExtraAttackCount";

    public ExtraAttackCountVar(decimal baseValue)
        : base(DefaultName, baseValue)
    {
    }
}