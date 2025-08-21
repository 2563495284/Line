using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 三个并排K线图显示管理器
/// </summary>
public class TripleKLineDisplay : MonoBehaviour
{
    [Header("K线组件引用")]
    [SerializeField]
    private LineView oilLineView;

    [SerializeField]
    private LineView steelLineView;

    [SerializeField]
    private LineView cottonLineView;

    [Header("布局设置")]
    [SerializeField]
    private float spacing = 6f; // K线图之间的间距（1920x1080优化）

    [SerializeField]
    private Vector3 centerPosition = Vector3.zero; // 中心位置

    [Header("更新设置")]
    [SerializeField]
    private float updateInterval = 1f;

    [SerializeField]
    private bool autoUpdate = true;

    [Header("调试")]
    [SerializeField]
    private bool showDebugInfo = false;

    // 私有变量
    private Dictionary<EStockType, LineView> lineViewMap;
    private bool isInitialized = false;

    private void Start()
    {
        InitializeKLineViews();

        if (autoUpdate)
        {
            InvokeRepeating(nameof(UpdateAllKLines), 1f, updateInterval);
        }
    }

    private void OnDestroy()
    {
        if (autoUpdate)
        {
            CancelInvoke(nameof(UpdateAllKLines));
        }
    }

    #region Initialization

    /// <summary>
    /// 初始化K线视图
    /// </summary>
    private void InitializeKLineViews()
    {
        lineViewMap = new Dictionary<EStockType, LineView>
        {
            { EStockType.Oil, oilLineView },
            { EStockType.Steel, steelLineView },
            { EStockType.Cotton, cottonLineView },
        };

        // 为每个LineView设置股票类型信息
        if (oilLineView != null)
        {
            oilLineView.SetStockInfo(EStockType.Oil, "石油", new Color(0.2f, 0.2f, 0.2f));
        }

        if (steelLineView != null)
        {
            steelLineView.SetStockInfo(EStockType.Steel, "钢铁", new Color(0.7f, 0.7f, 0.7f));
        }

        if (cottonLineView != null)
        {
            cottonLineView.SetStockInfo(EStockType.Cotton, "棉花", new Color(0.9f, 0.9f, 0.8f));
        }

        isInitialized = true;

        if (showDebugInfo)
        {
            Debug.Log("三K线显示系统初始化完成");
        }
    }

    #endregion

    #region Update System

    /// <summary>
    /// 更新所有K线图
    /// </summary>
    public void UpdateAllKLines()
    {
        if (!isInitialized || MultiStockSystem.Instance == null)
            return;

        var stockMarkets = MultiStockSystem.Instance.GetAllStockMarkets();

        foreach (var market in stockMarkets)
        {
            UpdateKLine(market);
        }

        if (showDebugInfo)
        {
            Debug.Log("所有K线图更新完成");
        }
    }

    /// <summary>
    /// 更新单个K线图
    /// </summary>
    private void UpdateKLine(SingleStockMarketData marketData)
    {
        if (
            lineViewMap.TryGetValue(marketData.stockType, out LineView lineView)
            && lineView != null
        )
        {
            // 使用历史价格数据更新K线
            lineView.SetNewData(marketData.priceHistory);

            // 更新当前价格显示
            lineView.UpdateCurrentPrice(
                marketData.currentPrice,
                marketData.GetPriceChangePercent()
            );
        }
    }

    /// <summary>
    /// 添加新价格点
    /// </summary>
    public void AddNewPricePoint(EStockType stockType, float newPrice)
    {
        if (lineViewMap.TryGetValue(stockType, out LineView lineView) && lineView != null)
        {
            lineView.SetNewPrice(newPrice);
        }
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// 手动刷新所有显示
    /// </summary>
    [ContextMenu("刷新所有K线")]
    public void RefreshAllDisplays()
    {
        UpdateAllKLines();
    }

    /// <summary>
    /// 设置自动更新间隔
    /// </summary>
    public void SetUpdateInterval(float interval)
    {
        updateInterval = Mathf.Max(0.1f, interval);

        if (autoUpdate)
        {
            CancelInvoke(nameof(UpdateAllKLines));
            InvokeRepeating(nameof(UpdateAllKLines), 1f, updateInterval);
        }
    }

    /// <summary>
    /// 启用/禁用自动更新
    /// </summary>
    public void SetAutoUpdate(bool enabled)
    {
        autoUpdate = enabled;

        if (autoUpdate)
        {
            InvokeRepeating(nameof(UpdateAllKLines), 1f, updateInterval);
        }
        else
        {
            CancelInvoke(nameof(UpdateAllKLines));
        }
    }

    /// <summary>
    /// 获取指定股票的LineView
    /// </summary>
    public LineView GetLineView(EStockType stockType)
    {
        lineViewMap.TryGetValue(stockType, out LineView lineView);
        return lineView;
    }

    public LineView GetLineViewRandom()
    {
        return lineViewMap.Values.RandomElement();
    }

    /// <summary>
    /// 设置LineView引用（用于编辑器或代码配置）
    /// </summary>
    public void SetLineViewReferences(LineView oil, LineView steel, LineView cotton)
    {
        oilLineView = oil;
        steelLineView = steel;
        cottonLineView = cotton;

        // 重新初始化映射
        lineViewMap = new Dictionary<EStockType, LineView>
        {
            { EStockType.Oil, oilLineView },
            { EStockType.Steel, steelLineView },
            { EStockType.Cotton, cottonLineView },
        };

        // 重新初始化
        InitializeKLineViews();

        if (showDebugInfo)
        {
            Debug.Log("LineView引用已更新");
        }
    }

    /// <summary>
    /// 设置中心位置
    /// </summary>
    public void SetCenterPosition(Vector3 newCenter)
    {
        centerPosition = newCenter;
    }

    /// <summary>
    /// 设置间距
    /// </summary>
    public void SetSpacing(float newSpacing)
    {
        spacing = newSpacing;
    }

    /// <summary>
    /// 清空所有K线图
    /// </summary>
    [ContextMenu("清空所有K线")]
    public void ClearAllKLines()
    {
        foreach (var lineView in lineViewMap.Values)
        {
            if (lineView != null)
            {
                lineView.ClearChart();
            }
        }
    }

    #endregion

    #region Event Handlers

    private void OnEnable() { }

    private void OnDisable() { }
    #endregion

    #region Debug Methods

    /// <summary>
    /// 打印当前状态
    /// </summary>
    [ContextMenu("打印当前状态")]
    public void PrintCurrentStatus()
    {
        Debug.Log("=== 三K线显示状态 ===");
        Debug.Log($"初始化状态: {isInitialized}");
        Debug.Log($"自动更新: {autoUpdate}");
        Debug.Log($"更新间隔: {updateInterval}秒");
        Debug.Log($"间距: {spacing}");
        Debug.Log($"中心位置: {centerPosition}");

        foreach (var kvp in lineViewMap)
        {
            bool hasView = kvp.Value != null;
            Debug.Log($"{kvp.Key}: {(hasView ? "已连接" : "未连接")}");
        }
    }

    #endregion
}
