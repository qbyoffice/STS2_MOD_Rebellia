using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Rebellia.RebelliaCode.Api.DynamicVars;

public class ThresholdVar(decimal baseValue) : DynamicVar(DefaultName, baseValue)
{
    public const string DefaultName = "Threshold";
}