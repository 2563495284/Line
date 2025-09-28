using System.Collections.Generic;
using UnityEngine;
using GameConfig;
public static class NotifyConst
{
    public static readonly string
        UpdatePlayerAttr = "UpdatePlayerAttr",
        UpdateManaUI = "UpdateManaUI",
        UpdateMoneyUI = "UpdateMoneyUI",
        UpdateStockChart = "UpdateStockChart",
        CancelPreviewCard = "CancelPreviewCard",//PreviewCardArgs
        PreviewCard = "PreviewCard",//PreviewCardArgs
        SelectCardTarget = "SelectCardTarget",//CardTargetArgs
        CancelSelectCardTarget = "CancelSelectTargetCard",//CardTargetArgs
        StartDragCard = "StartDragCard",//DragCardArgs
        CancelSelectCard = "CancelSelectCard",//DragCardArgs
        DraggingCard = "DraggingCard",//DragCardArgs
        /// <summary> 参数：SetPointStateArgs </summary>
        SetPointState = "SetPointState",
        /// <summary> 参数：ShowSidenoteArgs </summary>
        ShowSidenote = "ShowSidenote",
        HideSidenote = "HideSidenote",
        UpdateStockInfo = "UpdateStockInfo",
        UpdateNewsHistory = "UpdateNewsHistory",
        PopupNews = "PopupNews",
        PopupTips = "PopupTips";
}
public class PreviewCardArgs
{
    public CardModel card;
}
public class SetPointStateArgs
{
    public int stockId;
    public PointState state;
}
public class CardTargetArgs
{
    public CardModel card;
    public ICardEffectTarget target;
}
public class ShowSidenoteArgs
{
    public List<AttrType> types = new();
    public List<AttrType> highlights = new();
    public Vector3 worldPos;
}
public class DragCardArgs
{
    public CardModel card;
    public Vector3 worldPos;
}
public class PopupNewsArgs
{
    public string title;
    public string content;
    public NewsType newsType = NewsType.Info;
}
public class PopupTipsArgs
{
    public string message = "";
    public TipsType tipsType = TipsType.Info;
    public float duration = -1;
}