using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 多股市显示UI
/// </summary>
public class MultiStockDisplay : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Transform stockItemContainer;
    [SerializeField] private GameObject stockItemPrefab;

    [Header("总览显示")]
    [SerializeField] private TextMeshProUGUI totalAssetsText;
    [SerializeField] private TextMeshProUGUI currentMoneyText;

    [Header("更新设置")]
    [SerializeField] private float updateInterval = 1f;

    private List<StockDisplayItem> stockDisplayItems = new List<StockDisplayItem>();
    private Coroutine updateCoroutine;

    private void Start()
    {
        InitializeStockDisplay();
        StartUpdateCoroutine();
    }

    private void OnDestroy()
    {
        StopUpdateCoroutine();
    }

    #region Initialization

    /// <summary>
    /// 初始化股票显示
    /// </summary>
    private void InitializeStockDisplay()
    {
        if (MultiStockSystem.Instance == null)
        {
            Debug.LogError("MultiStockSystem未找到！");
            return;
        }

        var stockMarkets = MultiStockSystem.Instance.GetAllStockMarkets();

        foreach (var market in stockMarkets)
        {
            CreateStockDisplayItem(market);
        }
    }

    /// <summary>
    /// 创建股票显示项
    /// </summary>
    private void CreateStockDisplayItem(SingleStockMarketData marketData)
    {
        if (stockItemPrefab == null || stockItemContainer == null)
        {
            Debug.LogError("缺少必要的UI组件引用！");
            return;
        }

        GameObject itemObj = Instantiate(stockItemPrefab, stockItemContainer);
        StockDisplayItem displayItem = itemObj.GetComponent<StockDisplayItem>();

        if (displayItem != null)
        {
            displayItem.Initialize(marketData);
            stockDisplayItems.Add(displayItem);
        }
        else
        {
            Debug.LogError("StockItemPrefab缺少StockDisplayItem组件！");
            Destroy(itemObj);
        }
    }

    #endregion

    #region Update System

    /// <summary>
    /// 开始更新协程
    /// </summary>
    private void StartUpdateCoroutine()
    {
        if (updateCoroutine == null)
        {
            updateCoroutine = StartCoroutine(UpdateDisplayCoroutine());
        }
    }

    /// <summary>
    /// 停止更新协程
    /// </summary>
    private void StopUpdateCoroutine()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
    }

    /// <summary>
    /// 更新显示协程
    /// </summary>
    private IEnumerator UpdateDisplayCoroutine()
    {
        while (true)
        {
            UpdateAllDisplays();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    /// <summary>
    /// 更新所有显示
    /// </summary>
    public void UpdateAllDisplays()
    {
        if (MultiStockSystem.Instance == null) return;

        // 更新各个股票显示
        foreach (var displayItem in stockDisplayItems)
        {
            displayItem.UpdateDisplay();
        }

        // 更新总览信息
        UpdateOverviewDisplay();
    }

    /// <summary>
    /// 更新总览显示
    /// </summary>
    private void UpdateOverviewDisplay()
    {
        if (MultiStockSystem.Instance == null) return;

        float currentMoney = MultiStockSystem.Instance.GetCurrentMoney();
        float totalAssets = MultiStockSystem.Instance.GetTotalAssetValue();

        if (currentMoneyText != null)
        {
            currentMoneyText.text = $"现金: ¥{currentMoney:F2}";
        }

        if (totalAssetsText != null)
        {
            totalAssetsText.text = $"总资产: ¥{totalAssets:F2}";
        }
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// 手动刷新显示
    /// </summary>
    [ContextMenu("刷新显示")]
    public void RefreshDisplay()
    {
        UpdateAllDisplays();
    }

    /// <summary>
    /// 设置更新间隔
    /// </summary>
    public void SetUpdateInterval(float interval)
    {
        updateInterval = Mathf.Max(0.1f, interval);
    }

    #endregion
}
