using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 股票系统 - 处理股票价格变化和K线图更新
/// </summary>
public class StockSystem : Singleton<StockSystem>
{
    [Header("股票设置")]
    [SerializeField] private LineUI lineUI;
    [SerializeField] private float initialStockPrice = 100f;
    [SerializeField] private float minStockPrice = 1f;
    [SerializeField] private float maxStockPrice = 1000f;

    private float currentStockPrice;
    private List<float> priceHistory = new List<float>();
    private Coroutine priceRefreshCoroutine;
    [SerializeField] private float priceRefreshInterval = 3f; // 价格刷新间隔（秒）

    public float CurrentStockPrice => currentStockPrice;
    public List<float> PriceHistory => new List<float>(priceHistory);

    public float ViewStockPrice => lineUI.ViewPrice;
    protected override void Awake()
    {
        base.Awake();
        currentStockPrice = initialStockPrice;
        priceHistory.Add(currentStockPrice);
        lineUI.SetNewPrice(currentStockPrice);

        // 启动价格刷新协程
        StartPriceRefreshCoroutine();
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<RaiseStockPriceGA>(RaiseStockPricePerformer);
        ActionSystem.AttachPerformer<LowerStockPriceGA>(LowerStockPricePerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<RaiseStockPriceGA>();
        ActionSystem.DetachPerformer<LowerStockPriceGA>();
        ActionSystem.DetachPerformer<ChangeStockValueGA>();

        // 停止价格刷新协程
        StopPriceRefreshCoroutine();
    }

    #region Performers
    /// <summary>
    /// 处理股票价格上涨
    /// </summary>
    private IEnumerator RaiseStockPricePerformer(RaiseStockPriceGA action)
    {
        float oldPrice = currentStockPrice;

        // 计算新的价格
        float priceIncrease = action.RaisePrice;
        if (action.RaisePersent > 0)
        {
            priceIncrease += currentStockPrice * action.RaisePersent;
        }

        currentStockPrice += priceIncrease;

        // 限制价格范围
        currentStockPrice = Mathf.Clamp(currentStockPrice, minStockPrice, maxStockPrice);

        // 更新价格历史
        UpdatePriceHistory();

        // 通知其他系统
        NotifyPriceChange(oldPrice, currentStockPrice);

        Debug.Log($"股票价格上涨: {oldPrice:F2} -> {currentStockPrice:F2} (+{priceIncrease:F2})");

        yield return null;
    }

    /// <summary>
    /// 处理股票价格下跌
    /// </summary>
    private IEnumerator LowerStockPricePerformer(LowerStockPriceGA action)
    {
        float oldPrice = currentStockPrice;

        // 计算新的价格
        float priceDecrease = action.LowerPrice;
        if (action.LowerPersent > 0)
        {
            priceDecrease += currentStockPrice * action.LowerPersent;
        }

        currentStockPrice -= priceDecrease;

        // 限制价格范围
        currentStockPrice = Mathf.Clamp(currentStockPrice, minStockPrice, maxStockPrice);

        // 更新价格历史
        UpdatePriceHistory();

        // 通知其他系统
        NotifyPriceChange(oldPrice, currentStockPrice);

        Debug.Log($"股票价格下跌: {oldPrice:F2} -> {currentStockPrice:F2} (-{priceDecrease:F2})");

        yield return null;
    }

    /// <summary>
    /// 处理股票价格变化（通用）
    /// </summary>
    private IEnumerator ChangeStockValuePerformer(ChangeStockValueGA action)
    {
        float oldPrice = currentStockPrice;
        currentStockPrice += action.Value;

        // 限制价格范围
        currentStockPrice = Mathf.Clamp(currentStockPrice, minStockPrice, maxStockPrice);

        // 更新价格历史
        UpdatePriceHistory();

        // 通知其他系统
        NotifyPriceChange(oldPrice, currentStockPrice);

        string changeType = action.Value >= 0 ? "上涨" : "下跌";
        Debug.Log($"股票价格{changeType}: {oldPrice:F2} -> {currentStockPrice:F2} ({action.Value:F2})");

        yield return null;
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// 更新价格历史
    /// </summary>
    private void UpdatePriceHistory()
    {
        priceHistory.Add(currentStockPrice);

        // 限制历史记录数量，避免内存过多
        if (priceHistory.Count > 1000)
        {
            priceHistory.RemoveAt(0);
        }
    }

    /// <summary>
    /// 通知其他系统价格变化
    /// </summary>
    private void NotifyPriceChange(float oldPrice, float newPrice)
    {
        // 可以在这里添加其他系统的通知
        // 例如：通知UI系统、通知音效系统等
    }

    /// <summary>
    /// 启动价格刷新协程
    /// </summary>
    private void StartPriceRefreshCoroutine()
    {
        if (priceRefreshCoroutine != null)
        {
            StopCoroutine(priceRefreshCoroutine);
        }

        priceRefreshCoroutine = StartCoroutine(PriceRefreshCoroutine());
        Debug.Log("价格刷新协程已启动");
    }

    /// <summary>
    /// 停止价格刷新协程
    /// </summary>
    private void StopPriceRefreshCoroutine()
    {
        if (priceRefreshCoroutine != null)
        {
            StopCoroutine(priceRefreshCoroutine);
            priceRefreshCoroutine = null;
        }

        Debug.Log("价格刷新协程已停止");
    }

    /// <summary>
    /// 价格刷新协程
    /// </summary>
    private IEnumerator PriceRefreshCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(priceRefreshInterval);

            // 刷新价格到UI
            if (lineUI != null)
            {
                lineUI.SetNewPrice(currentStockPrice);
                Debug.Log($"价格刷新到UI: {currentStockPrice:F2}");
            }
        }
    }

    /// <summary>
    /// 手动设置股票价格（用于测试）
    /// </summary>
    public void SetStockPrice(float newPrice)
    {
        float oldPrice = currentStockPrice;
        currentStockPrice = Mathf.Clamp(newPrice, minStockPrice, maxStockPrice);

        UpdatePriceHistory();
        NotifyPriceChange(oldPrice, currentStockPrice);

        Debug.Log($"手动设置股票价格: {oldPrice:F2} -> {currentStockPrice:F2}");
    }

    /// <summary>
    /// 获取价格变化百分比
    /// </summary>
    public float GetPriceChangePercent()
    {
        if (priceHistory.Count < 2) return 0f;

        float previousPrice = priceHistory[priceHistory.Count - 2];
        if (previousPrice == 0) return 0f;

        return ((currentStockPrice - previousPrice) / previousPrice) * 100f;
    }

    /// <summary>
    /// 重置股票价格
    /// </summary>
    public void ResetStockPrice()
    {
        float oldPrice = currentStockPrice;
        currentStockPrice = initialStockPrice;

        priceHistory.Clear();
        priceHistory.Add(currentStockPrice);

        NotifyPriceChange(oldPrice, currentStockPrice);

        Debug.Log($"重置股票价格: {oldPrice:F2} -> {currentStockPrice:F2}");
    }
    #endregion

    #region Public Methods
    public bool HasEnoughMoney(int buyStockAmount, float money)
    {
        return money >= buyStockAmount * ViewStockPrice;
    }
    /// <summary>
    /// 获取当前股票价格
    /// </summary>
    public float GetCurrentStockPrice()
    {
        return currentStockPrice;
    }
    #endregion
}