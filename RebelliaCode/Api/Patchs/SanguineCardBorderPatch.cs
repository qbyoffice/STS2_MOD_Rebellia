using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Rebellia.RebelliaCode.Api.Extensions;

namespace Rebellia.RebelliaCode.Patches;

public class SanguineCardBorderPatch
{
    private const string BorderNodeName = "RebelliaSanguineBorder";
    private const string BorderScenePath = "res://Rebellia/scenes/cards/RebelliaSanguine.tscn";

    [HarmonyPatch(typeof(NCard))]
    public static class Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("_Ready")]
        public static void OnReady(NCard __instance) => UpdateBorder(__instance);

        [HarmonyPostfix]
        [HarmonyPatch("UpdateVisuals")]
        public static void OnUpdateVisuals(
            NCard __instance,
            PileType pileType,
            CardPreviewMode previewMode
        ) => UpdateBorder(__instance);

        [HarmonyPostfix]
        [HarmonyPatch("Reload")]
        public static void OnReload(NCard __instance) => UpdateBorder(__instance);
    }

    private static void UpdateBorder(NCard cardNode)
    {
        if (cardNode?.Model == null || !cardNode.IsNodeReady())
            return;

        bool hasKeyword = cardNode.Model.Keywords.Contains(RCardKeywordExtensions.RebelliaSanguine);
        bool isPlayPile = cardNode.Model.Pile?.Type == PileType.Play;
        bool shouldShow = hasKeyword && !isPlayPile;

        Control existing = cardNode.GetNodeOrNull<Control>(BorderNodeName);

        if (shouldShow)
        {
            if (existing == null)
            {
                // 使用 PreloadManager.Cache.GetScene 代替 GD.Load
                PackedScene borderScene = PreloadManager.Cache.GetScene(BorderScenePath);
                if (borderScene == null)
                {
                    GD.PrintErr(
                        $"[SanguineBorder] Failed to load scene via PreloadManager: {BorderScenePath}"
                    );
                    return;
                }

                Control border = borderScene.Instantiate<Control>();
                border.Name = BorderNodeName;
                border.MouseFilter = Control.MouseFilterEnum.Ignore; // 鼠标穿透

                // 将边框添加到 OverlayContainer（官方预留层），如果不存在则添加到 CardContainer
                Control parent = cardNode.GetNodeOrNull<Control>("CardContainer/OverlayContainer");
                if (parent == null)
                    parent = cardNode.GetNodeOrNull<Control>("CardContainer");
                if (parent == null)
                    parent = cardNode;

                parent.AddChild(border);
                parent.MoveChild(border, 0); // 移到底层

                border.AnchorLeft = 0;
                border.AnchorTop = 0;
                border.AnchorRight = 1;
                border.AnchorBottom = 1;
                border.Size = cardNode.Size;
                border.ZIndex = 100; // 确保在底层但可见（数值不宜过高以免遮挡文字，但这里父节点其他元素ZIndex较低）
                SetChildZIndex(border, 100);

                AnimationPlayer anim = border.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
                if (anim == null)
                    anim = border.GetNodeOrNull<AnimationPlayer>("VisualLayer2/AnimationPlayer");
                anim?.Play("RebelliaSanguine");
            }
            else
            {
                existing.Visible = true;
            }
        }
        else if (existing != null)
        {
            existing.QueueFree();
        }
    }

    private static void SetChildZIndex(Node node, int zIndex)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is CanvasItem canvasItem)
                canvasItem.ZIndex = zIndex;
            SetChildZIndex(child, zIndex);
        }
    }
}
