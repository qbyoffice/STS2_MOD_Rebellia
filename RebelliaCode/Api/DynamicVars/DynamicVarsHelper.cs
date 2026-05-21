using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Api.DynamicVars;

public static class DynamicVarsHelper
{
    public static PowerVar<T> GetPowerVar<T>(DynamicVarSet varSet)
        where T : PowerModel
    {
        return (PowerVar<T>)varSet[typeof(T).Name];
    }

    public static DiscardsVar GetDiscardsVar(DynamicVarSet varSet)
    {
        return (DiscardsVar)varSet[DiscardsVar.DefaultName];
    }

    public static ThresholdVar GetThresholdVar(DynamicVarSet varSet)
    {
        return (ThresholdVar)varSet[ThresholdVar.DefaultName];
    }

    public static PowerVar<BloodSwordArtPower> GetBloodSwordArtVar(this DynamicVarSet varSet)
    {
        return (PowerVar<BloodSwordArtPower>)varSet[typeof(BloodSwordArtPower).Name];
    }
}