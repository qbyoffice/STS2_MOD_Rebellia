using System.Reflection;
using BaseLib.Utils.NodeFactories;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Rebellia.RebelliaCode.Api.Combat;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Rebellia.RebelliaCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Rebellia"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    private static void InitializeTools()
    {
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
    }

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        InitializeTools();
        harmony.PatchAll();
        NodeFactory.RegisterSceneType<RNHealthBar>("res://scenes/combat/Rebellia_health_bar.tscn");
    }
}
