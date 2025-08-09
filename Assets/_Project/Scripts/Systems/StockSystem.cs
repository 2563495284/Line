using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 股票系统 - 处理股票价格变化和K线图更新
/// </summary>
public class StockSystem : Singleton<StockSystem>
{
    private const int MAX_PRICE_HISTORY = 40; // 最大价格历史记录数

    [Header("股票设置")]
    [SerializeField] private LineView lineView;
    [SerializeField] private float initialStockPrice = 100f;
    [SerializeField] private float minStockPrice = 1f;
    [SerializeField] private float maxStockPrice = 1000f;

    [Header("账户")]
    [SerializeField] public MoneyUI moneyUI;
    [SerializeField] public StockPriceDisplay stockPriceDisplay;
    [SerializeField] float initialMoney = 10000;

    private float currentMoney;
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
        moneyUI.UpdateStockText(stockCount);
        moneyUI.UpdateAllValuesText(stockCount * nextStockPrice);
        stockPriceDisplay.UpdatePrice(nextStockPrice);
    }
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<BuyStockGA>(BuyStockPerformer);
        ActionSystem.AttachPerformer<SellStockGA>(SellStockPerformer);
        ActionSystem.AttachPerformer<ChangeStockPriceGA>(ChangeStockPricePerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<BuyStockGA>();
        ActionSystem.DetachPerformer<SellStockGA>();
        ActionSystem.DetachPerformer<ChangeStockPriceGA>();

        // 停止价格刷新协程
        StopPriceRefreshCoroutine();
    }

    #region Performers
    private IEnumerator BuyStockPerformer(BuyStockGA action)
    {
        currentMoney -= action.Amount * CurrentStockPrice;
        stockCount += action.Amount;
        moneyUI.UpdateMoneyText(currentMoney);
        moneyUI.UpdateStockText(stockCount);
        moneyUI.UpdateAllValuesText(CurrentStockPrice);
        yield return null;
    }

    private IEnumerator SellStockPerformer(SellStockGA action)
    {
        currentMoney += action.Amount * CurrentStockPrice;
        stockCount -= action.Amount;
        moneyUI.UpdateMoneyText(currentMoney);
        moneyUI.UpdateStockText(stockCount);
        moneyUI.UpdateAllValuesText(CurrentStockPrice);
        yield return null;
    }
    /// <summary>
    /// 处理股票价格上涨
    /// </summary>
    private IEnumerator ChangeStockPricePerformer(ChangeStockPriceGA action)
    {
        float oldPrice = nextStockPrice;
        ECharacterStrategyType characterStrategyType = action.characterView.StrategyType;
        int index = (int)characterStrategyType;
        float changeStockPrice = 0;
        float changeStockPersentPrice = 0;
        action.ChangePriceDictionary.TryGetValue(characterStrategyType, out changeStockPrice);
        action.ChangePricePersentDictionary.TryGetValue(characterStrategyType, out changeStockPersentPrice);
        // 计算新的价格
        float priceIncrease = changeStockPrice;
        priceIncrease += Mathf.Round(nextStockPrice * changeStockPersentPrice) / 100f;

        nextStockPrice = Mathf.Round((nextStockPrice + priceIncrease) * 100f) / 100f;

        // 限制价格范围并保留两位小数
        nextStockPrice = Mathf.Round(Mathf.Clamp(nextStockPrice, minStockPrice, maxStockPrice) * 100f) / 100f;

        // 更新价格历史
        UpdatePriceHistory();
        Debug.Log($"NPC {action.characterView.name} 投资策略: {characterStrategyType} 价格: {oldPrice:F2} -> {nextStockPrice:F2} ({priceIncrease:F2})");
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
        moneyUI.UpdateAllValuesText(stockCount * newPrice);
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
                NotifyPriceChange(oldPrice, nextStockPrice);
            }
        }
    }

    #endregion

    #region Public Methods
    public bool HasEnoughMoney(int buyStockAmount)
    {
        if (buyStockAmount <= 0)
        {
            return true;
        }
        return currentMoney >= buyStockAmount * CurrentStockPrice;
    }
    #endregion
}