using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 股票K线图显示组件
/// 支持实时数据更新、点状态管理、对象池优化
/// </summary>
public class LineView : MonoBehaviour
{
    #region 配置参数

    [Header("图表设置")]
    [SerializeField] private float chartWidth = 8f;
    [SerializeField] private float chartHeight = 5f;
    [SerializeField] private float padding = 0.3f;

    [Header("线条设置")]
    [SerializeField] private Color lineColor = Color.blue;

    [Header("点设置")]
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private float pointSize = 1f;
    [SerializeField] private Color pointColor = Color.red;
    [SerializeField] private int maxPoints = 30;

    [Header("背景设置")]
    [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    [SerializeField] private GameObject background;

    [Header("股票信息显示")]
    [SerializeField] private TextMeshPro titleText;
    [SerializeField] private TextMeshPro currentPriceText;
    [SerializeField] private TextMeshPro changePercentText;
    [SerializeField] private Color positiveColor = Color.green;
    [SerializeField] private Color negativeColor = Color.red;
    [SerializeField] private Color neutralColor = Color.white;

    #endregion

    #region 组件引用

    [SerializeField] private LineRenderer kLineRenderer;

    #endregion

    #region 私有字段

    // 数据存储
    private List<float> prices = new List<float>();

    // 点对象管理
    private List<GameObject> pointObjects = new List<GameObject>();

    // 状态管理
    private Dictionary<int, PointState> pointStates = new Dictionary<int, PointState>();

    // 股票信息
    private EStockType stockType;
    private string stockName;
    private Color themeColor;

    #endregion

    #region 公共属性

    public EStockType StockType => stockType;
    public float ViewPrice => prices.Count > 0 ? prices[prices.Count - 1] : 0;

    #endregion

    #region Unity生命周期

    void Start()
    {
        InitializeComponents();
    }

    void OnDestroy()
    {
        CleanupResources();
    }

    #endregion

    #region 初始化方法

    /// <summary>
    /// 初始化所有组件
    /// </summary>
    private void InitializeComponents()
    {
        InitializeBackground();
        InitializeStockDisplay();
    }

    /// <summary>
    /// 初始化背景
    /// </summary>
    private void InitializeBackground()
    {
        if (background == null) return;

        SpriteRenderer bgRenderer = background.GetComponent<SpriteRenderer>();
        if (bgRenderer?.sprite == null) return;

        Sprite sprite = bgRenderer.sprite;
        float scaleX = chartWidth / sprite.bounds.size.x;
        float scaleY = chartHeight / sprite.bounds.size.y;
        background.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    /// <summary>
    /// 初始化股票信息显示
    /// </summary>
    private void InitializeStockDisplay()
    {
        UpdateTitleDisplay();
        UpdateLineRendererColor();
    }

    #endregion

    #region 股票信息管理

    /// <summary>
    /// 设置股票信息
    /// </summary>
    public void SetStockInfo(EStockType type, string name, Color color)
    {
        stockType = type;
        stockName = name;
        themeColor = color;
        InitializeStockDisplay();
    }

    /// <summary>
    /// 更新当前价格显示
    /// </summary>
    public void UpdateCurrentPrice(float currentPrice, float changePercent)
    {
        UpdatePriceText(currentPrice);
        UpdateChangePercentText(changePercent);
    }

    private void UpdateTitleDisplay()
    {
        if (titleText != null)
        {
            titleText.text = stockName ?? "股票";
        }
    }

    private void UpdateLineRendererColor()
    {
        if (kLineRenderer == null) return;

        Color targetColor = lineColor;
        kLineRenderer.startColor = targetColor;
        kLineRenderer.endColor = targetColor;
    }

    private void UpdatePriceText(float currentPrice)
    {
        if (currentPriceText != null)
        {
            currentPriceText.text = $"¥{currentPrice:F2}";
        }
    }

    private void UpdateChangePercentText(float changePercent)
    {
        if (changePercentText == null) return;

        string changeSymbol = changePercent > 0 ? "+" : "";
        changePercentText.text = $"{changeSymbol}{changePercent:F1}%";

        Color textColor = GetChangeColor(changePercent);
        changePercentText.color = textColor;
    }

    private Color GetChangeColor(float changePercent)
    {
        if (changePercent > 0) return positiveColor;
        if (changePercent < 0) return negativeColor;
        return neutralColor;
    }

    #endregion

    #region 数据管理

    /// <summary>
    /// 设置新的价格数据
    /// </summary>
    public void SetNewData(List<float> newPrices)
    {
        if (!ValidatePriceData(newPrices)) return;

        List<float> validPrices = FilterValidPrices(newPrices);
        if (validPrices.Count == 0) return;

        if (validPrices.Count > maxPoints)
        {
            int startIndex = validPrices.Count - maxPoints;
            validPrices = validPrices.GetRange(startIndex, maxPoints);
            AdjustStateIndices(startIndex);
        }

        prices = validPrices;
        UpdateChart();
    }

    /// <summary>
    /// 添加新价格
    /// </summary>
    public void SetNewPrice(float newPrice)
    {
        prices.Add(newPrice);

        if (prices.Count > maxPoints)
        {
            prices.RemoveAt(0);
            AdjustStateIndicesForNewPrice();
        }

        UpdateChart();
    }

    /// <summary>
    /// 清除图表
    /// </summary>
    public void ClearChart()
    {
        prices.Clear();
        ClearAllPointStates();
        UpdateChart();
    }

    private bool ValidatePriceData(List<float> newPrices)
    {
        if (newPrices == null)
        {
            Debug.LogError("传入的价格数据为null");
            return false;
        }
        return true;
    }

    private List<float> FilterValidPrices(List<float> newPrices)
    {
        List<float> validPrices = new List<float>();

        for (int i = 0; i < newPrices.Count; i++)
        {
            if (float.IsNaN(newPrices[i]) || float.IsInfinity(newPrices[i]))
            {
                Debug.LogWarning($"跳过无效价格数据 [{i}]: {newPrices[i]}");
                continue;
            }
            validPrices.Add(newPrices[i]);
        }

        if (validPrices.Count == 0)
        {
            Debug.LogWarning("没有有效的价格数据");
        }

        return validPrices;
    }

    #endregion

    #region 图表更新

    /// <summary>
    /// 更新图表显示
    /// </summary>
    public void UpdateChart()
    {
        if (prices.Count == 0) return;

        UpdateLineRenderer();
        UpdateAllPoints();
    }

    private void UpdateLineRenderer()
    {
        if (kLineRenderer == null) return;

        var priceRange = CalculatePriceRange();
        var positions = CalculatePointPositions(priceRange);

        kLineRenderer.positionCount = prices.Count;
        kLineRenderer.SetPositions(positions);
    }

    private (float minPrice, float maxPrice) CalculatePriceRange()
    {
        float minPrice = float.MaxValue;
        float maxPrice = float.MinValue;

        foreach (float price in prices)
        {
            minPrice = Mathf.Min(minPrice, price);
            maxPrice = Mathf.Max(maxPrice, price);
        }

        // 处理边界情况
        if (prices.Count == 1)
        {
            minPrice = prices[0] - 1f;
            maxPrice = prices[0] + 1f;
        }
        else if (Mathf.Approximately(minPrice, maxPrice))
        {
            minPrice -= 1f;
            maxPrice += 1f;
        }

        // 添加边距
        float priceRange = maxPrice - minPrice;
        if (priceRange < 1f) priceRange = 1f;
        minPrice -= priceRange * 0.1f;
        maxPrice += priceRange * 0.1f;

        return (minPrice, maxPrice);
    }

    private Vector3[] CalculatePointPositions((float minPrice, float maxPrice) priceRange)
    {
        Vector3[] positions = new Vector3[prices.Count];

        for (int i = 0; i < prices.Count; i++)
        {
            positions[i] = CalculatePointPosition(i, priceRange);
        }

        return positions;
    }

    private Vector3 CalculatePointPosition(int index, (float minPrice, float maxPrice) priceRange)
    {
        float normalizedX = CalculateNormalizedX(index);
        float normalizedY = Mathf.InverseLerp(priceRange.minPrice, priceRange.maxPrice, prices[index]);

        float x = Mathf.Lerp(-chartWidth / 2 + padding, chartWidth / 2 - padding, normalizedX);
        float y = Mathf.Lerp(-chartHeight / 2 + padding, chartHeight / 2 - padding, normalizedY);

        // 验证坐标值
        if (float.IsNaN(x) || float.IsNaN(y))
        {
            Debug.LogWarning($"检测到NaN坐标值，跳过点 {index}。价格: {prices[index]}, X: {x}, Y: {y}");
            return Vector3.zero;
        }

        return new Vector3(x, y, 0);
    }

    private float CalculateNormalizedX(int index)
    {
        if (prices.Count == 1) return 0.0f;
        return (float)index / (prices.Count - 1);
    }

    private void UpdateAllPoints()
    {
        var priceRange = CalculatePriceRange();

        // 确保有足够的点对象
        EnsurePointObjects(prices.Count);

        // 更新所有点对象
        for (int i = 0; i < prices.Count; i++)
        {
            Vector3 position = CalculatePointPosition(i, priceRange);
            if (position != Vector3.zero)
            {
                UpdatePointObject(i, position, prices[i]);
            }
        }

        // 隐藏多余的点对象
        for (int i = prices.Count; i < pointObjects.Count; i++)
        {
            if (pointObjects[i] != null)
            {
                pointObjects[i].SetActive(false);
            }
        }
    }

    #endregion

    #region 点对象管理

    private void EnsurePointObjects(int count)
    {
        // 创建缺少的点对象
        while (pointObjects.Count < count)
        {
            GameObject pointObj = Instantiate(pointPrefab, transform);
            pointObj.name = $"Point_{pointObjects.Count}";
            pointObjects.Add(pointObj);
        }
    }

    private void UpdatePointObject(int index, Vector3 position, float price)
    {
        if (index >= pointObjects.Count) return;

        GameObject pointObj = pointObjects[index];
        if (pointObj == null) return;

        Point point = pointObj.GetComponent<Point>();
        if (point == null) return;

        // 激活点对象
        pointObj.SetActive(true);

        // 确保状态标签正确初始化
        point.InitializeStateLabel();

        // 更新位置和价格信息
        bool isDefaultVisible = (index == prices.Count - 1) || index == 0;
        point.RefreshPriceLabel(price, index, isDefaultVisible);
        point.transform.localPosition = position;
        point.SetColor(pointColor);
        point.SetSize(pointSize);

        // 恢复状态
        RestorePointState(point, index);
    }

    private void RestorePointState(Point point, int index)
    {
        if (pointStates.ContainsKey(index))
        {
            PointState savedState = pointStates[index];
            point.SetState(savedState);
        }
        else
        {
            point.ClearState();
        }
    }

    #endregion

    #region 状态管理

    /// <summary>
    /// 设置指定点的状态
    /// </summary>
    public void SetPointState(int index, PointState state)
    {
        pointStates[index] = state;

        if (index < pointObjects.Count && pointObjects[index] != null)
        {
            pointObjects[index].GetComponent<Point>().SetState(state);
        }
    }

    /// <summary>
    /// 设置最新点的状态
    /// </summary>
    public void SetPointState(PointState state)
    {
        if (prices.Count > 0)
        {
            SetPointState(prices.Count - 1, state);
        }
    }

    /// <summary>
    /// 获取指定点的状态
    /// </summary>
    public PointState GetPointState(int index)
    {
        return pointStates.ContainsKey(index) ? pointStates[index] : PointState.None;
    }

    /// <summary>
    /// 获取最新点的状态
    /// </summary>
    public PointState GetLatestPointState()
    {
        return prices.Count > 0 ? GetPointState(prices.Count - 1) : PointState.None;
    }

    /// <summary>
    /// 清除指定点的状态
    /// </summary>
    public void ClearPointState(int index)
    {
        if (pointStates.ContainsKey(index))
        {
            pointStates.Remove(index);

            if (index < pointObjects.Count && pointObjects[index] != null)
            {
                pointObjects[index].GetComponent<Point>().ClearState();
            }
        }
    }

    /// <summary>
    /// 清除所有点的状态
    /// </summary>
    public void ClearAllPointStates()
    {
        pointStates.Clear();

        foreach (var pointObj in pointObjects)
        {
            if (pointObj != null)
            {
                pointObj.GetComponent<Point>().ClearState();
            }
        }
    }

    /// <summary>
    /// 获取所有有状态的点的索引
    /// </summary>
    public List<int> GetPointsWithStates()
    {
        return new List<int>(pointStates.Keys);
    }

    /// <summary>
    /// 调试方法：打印所有点的状态信息
    /// </summary>
    public void DebugPointStates()
    {
        Debug.Log($"=== 点状态调试信息 ===");
        Debug.Log($"总点数: {prices.Count}");
        Debug.Log($"点对象数: {pointObjects.Count}");
        Debug.Log($"保存的状态数: {pointStates.Count}");

        foreach (var kvp in pointStates)
        {
            Debug.Log($"点 {kvp.Key}: 状态 = {kvp.Value}");
        }

        for (int i = 0; i < pointObjects.Count; i++)
        {
            if (pointObjects[i] != null)
            {
                Point point = pointObjects[i].GetComponent<Point>();
                if (point != null)
                {
                    Debug.Log($"点对象 {i}: 当前状态 = {point.GetCurrentState()}");
                }
            }
        }
        Debug.Log($"========================");
    }

    #endregion

    #region 状态索引调整

    private void AdjustStateIndicesForNewPrice()
    {
        Dictionary<int, PointState> newStates = new Dictionary<int, PointState>();

        foreach (var kvp in pointStates)
        {
            int oldIndex = kvp.Key;
            PointState state = kvp.Value;
            int newIndex = oldIndex - 1;

            if (newIndex >= 0 && newIndex < maxPoints)
            {
                newStates[newIndex] = state;
            }
        }

        pointStates = newStates;
    }

    private void AdjustStateIndices(int startIndex)
    {
        Dictionary<int, PointState> newStates = new Dictionary<int, PointState>();

        foreach (var kvp in pointStates)
        {
            int oldIndex = kvp.Key;
            PointState state = kvp.Value;
            int newIndex = oldIndex - startIndex;

            if (newIndex >= 0 && newIndex < maxPoints)
            {
                newStates[newIndex] = state;
            }
        }

        pointStates = newStates;
    }

    #endregion

    #region 资源清理

    private void CleanupResources()
    {
        ClearPointObjects();
    }

    private void ClearPointObjects()
    {
        foreach (var pointObj in pointObjects)
        {
            if (pointObj != null)
            {
                DestroyImmediate(pointObj);
            }
        }
        pointObjects.Clear();
    }

    #endregion
}