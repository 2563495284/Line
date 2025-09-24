using System.Collections.Generic;
using UnityEngine;
public static class NotifyConst
{
    public static readonly string
        UpdatePlayerAttr = "UpdatePlayerAttr",
        UpdateManaUI = "UpdateManaUI",
        UpdateMoneyUI = "UpdateMoneyUI",
        UpdateStockChart = "UpdateStockChart",
        PredictStock = "PredictStock",
        TipsCardSelectFail = "TipsCardSelectFail",//TipsCardSelectFailArgs
        TipsCardReleaseFail = "TipsCardReleaseFail",//TipsCardReleaseFailArgs
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
    public EStockType stockType;
    public PointState state;
}
public class CardTargetArgs
{
    public CardModel card;
    public ICardEffectTarget target;
}
public class ShowSidenoteArgs
{
    public List<EAttrType> types = new();
    public List<EAttrType> highlights = new();
    public Vector3 worldPos;
}
public class PredictStockArgs
{
    public StockModel stock;
    public PredictionData predictionData;
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
public class TipsCardSelectFailArgs
{
    public CardModel card;
    public ECardSelectState failState;
}
public class TipsCardReleaseFailArgs
{
    public CardModel card;
    public IEffectReceiver target;
    public ECardReleaseState failState;
}