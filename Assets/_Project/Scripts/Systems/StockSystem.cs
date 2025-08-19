using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 股票系统 - 处理股票价格变化和K线图更新
/// </summary>
public class StockSystem : Singleton<StockSystem>
{
    private const int MAX_PRICE_HISTORY = 40; // 最大价格历史记录数

    [Header("股票设置")]
    [SerializeField] private LineView lineView;
    [SerializeField] public float initialStockPrice = 100f;
    [SerializeField] public float minStockPrice = 1f;
    [SerializeField] public float maxStockPrice = 1000f;

    [Header("账户")]
    [SerializeField] public MoneyUI moneyUI;
    [SerializeField] public StockPriceDisplay stockPriceDisplay;
    [SerializeField] float initialMoney = 10000;

    private float currentMoney;
    public float CurrentMoney => currentMoney;
    private int stockCount;

    //
    private float nextStockPrice;
    private List<float> priceHistory = new List<float>();
    private Coroutine priceRefreshCoroutine;
    [SerializeField] private float priceRefreshInterval = 3f; // 价格刷新间隔（秒）

    public float NextStockPrice => nextStockPrice;
    public List<float> PriceHistory => new List<float>(priceHistory);

    public float CurrentStockPrice => lineView.ViewPrice;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        nextStockPrice = Mathf.Round(initialStockPrice * 100f) / 100f;
        priceHistory.Add(nextStockPrice);
        lineView.SetNewPrice(nextStockPrice);

        // 启动价格刷新协程
        StartPriceRefreshCoroutine();

        currentMoney = initialMoney;
        moneyUI.UpdateMoneyText(currentMoney);
        // moneyUI.UpdateStockText(stockCount);
        // moneyUI.UpdateAllValuesText(nextStockPrice);
        stockPriceDisplay.UpdatePrice(nextStockPrice);
    }
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ChangeMoneyGA>(ChangeMoneyPerformer);
        ActionSystem.AttachPerformer<ChangeStockGA>(ChangeStockPerformer);
        ActionSystem.AttachPerformer<TradeAllStockGA>(TradeAllStockPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ChangeMoneyGA>();
        ActionSystem.DetachPerformer<ChangeStockGA>();
        ActionSystem.DetachPerformer<TradeAllStockGA>();
        // 停止价格刷新协程
        StopPriceRefreshCoroutine();
    }

    #region Performers
    // private IEnumerator TradeStockPerformer(TradeStockGA action)
    // {
    //     if (action.TradeAmount > 0)
    //     {
    //         int buyStockCount = math.min(action.TradeAmount, (int)math.floor(currentMoney / CurrentStockPrice));
    //         ChangeStockGA changeStockGA = new ChangeStockGA(buyStockCount, action.StockType);
    //         ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(-buyStockCount * CurrentStockPrice);
    //         ActionSystem.Instance.Perform(changeStockGA);
    //         ActionSystem.Instance.Perform(changeMoneyGA);
    //     }
    //     else
    //     {
    //         int sellStockCount = -math.min(-action.TradeAmount, stockCount);
    //         ChangeStockGA changeStockGA = new ChangeStockGA(sellStockCount, action.StockType);
    //         ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(-sellStockCount * CurrentStockPrice);
    //         ActionSystem.Instance.Perform(changeStockGA);
    //         ActionSystem.Instance.Perform(changeMoneyGA);
    //     }


    //     yield return null;
    // }

    /// <summary>
    /// 处理金币增减
    /// </summary>
    private IEnumerator ChangeMoneyPerformer(ChangeMoneyGA action)
    {
        currentMoney += action.Amount;
        moneyUI.UpdateMoneyText(currentMoney);
        // moneyUI.UpdateAllValuesText(CurrentStockPrice);

        Debug.Log($"金币变化: {(action.Amount > 0 ? "+" : "")}{action.Amount:F2} | 当前金币: {currentMoney:F2}");
        yield return null;
    }

    private IEnumerator ChangeStockPerformer(ChangeStockGA action)
    {
        stockCount = math.max(0, stockCount + action.Amount);
        // moneyUI.UpdateStockText(stockCount);
        // moneyUI.UpdateAllValuesText(CurrentStockPrice);
        Debug.Log($"股票变化: {(action.Amount > 0 ? "+" : "")}{action.Amount:F2} | 当前股票: {stockCount}");
        yield return null;
    }
    /// <summary>
    /// 梭哈/跑路
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator TradeAllStockPerformer(TradeAllStockGA action)
    {
        switch (action.TradeAllStockType)
        {
            case ETradeAllStockType.Buy:
                int couldBuyStockCount = (int)math.floor(currentMoney / CurrentStockPrice);
                if (couldBuyStockCount > 0)
                {
                    ChangeStockGA changeStockGA = new ChangeStockGA(couldBuyStockCount, action.StockType);
                    ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(-couldBuyStockCount * CurrentStockPrice);
                    ActionSystem.Instance.Perform(changeStockGA);
                    ActionSystem.Instance.Perform(changeMoneyGA);
                }
                break;
            case ETradeAllStockType.Sell:
                int couldSellStockCount = stockCount;
                if (couldSellStockCount > 0)
                {
                    ChangeStockGA changeStockGA = new ChangeStockGA(-couldSellStockCount, action.StockType);
                    ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(couldSellStockCount * CurrentStockPrice);
                    ActionSystem.Instance.Perform(changeStockGA);
                    ActionSystem.Instance.Perform(changeMoneyGA);
                }
                break;
        }
        yield return null;
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// 更新价格历史
    /// </summary>
    private void UpdatePriceHistory()
    {
        priceHistory.Add(nextStockPrice);

        // 限制历史记录数量，最多40条记录
        if (priceHistory.Count > MAX_PRICE_HISTORY)
        {
            priceHistory.RemoveAt(0);
        }
    }

    /// <summary>
    /// 通知其他系统价格变化
    /// </summary>
    private void NotifyPriceChange(float oldPrice, float newPrice)
    {
        // moneyUI.UpdateAllValuesText(newPrice);
        stockPriceDisplay.UpdatePrice(newPrice);
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
            if (lineView != null)
            {
                float oldPrice = CurrentStockPrice;
                lineView.SetNewPrice(nextStockPrice);
                NewsSystem.Instance.BroadcastStockPriceChange(oldPrice, nextStockPrice, "定期刷新");
                Debug.Log($"价格刷新到UI: {nextStockPrice:F2}");

                // 通知其他系统
                PredictionSystem.Instance.UpdatePredictionCountdown();

                // 更新Buff回合计数
                BuffSystem.Instance.UpdateBuffRounds();

                NotifyPriceChange(oldPrice, nextStockPrice);

            }
        }
    }

    #endregion

    #region Public Methods

    #endregion
}