using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Rebellia.RebelliaCode.Api.DynamicVars;

public class DiscardsVar(decimal baseValue) : DynamicVar(DefaultName, baseValue)
{
    public const string DefaultName = "Discards";
}