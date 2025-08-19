using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LineView : MonoBehaviour
{
    [Header("图表设置")]
    public float chartWidth = 8f;  // 适合三分屏显示
    public float chartHeight = 5f; // 保持16:10比例
    public float padding = 0.3f;  // 减少内边距以充分利用空间

    [Header("线条设置")]
    public float lineWidth = 0.08f;  // 略细的线条，适合较小显示
    public Color lineColor = Color.blue;

    [Header("点设置")]
    public GameObject pointPrefab;
    public float pointSize = 1f;  // 较小的点，避免拥挤
    public Color pointColor = Color.red;
    public bool showPriceLabels = true;
    public int maxPoints = 30; // 减少点数，适合较窄的显示区域

    [Header("背景设置")]
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    public GameObject background;

    [Header("股票信息显示")]
    [SerializeField] private TextMeshPro titleText;
    [SerializeField] private TextMeshPro currentPriceText;
    [SerializeField] private TextMeshPro changePercentText;
    [SerializeField] private Color positiveColor = Color.green;
    [SerializeField] private Color negativeColor = Color.red;
    [SerializeField] private Color neutralColor = Color.white;

    // 私有变量
    public LineRenderer kLineRenderer;
    private List<GameObject> points = new List<GameObject>();
    private List<float> prices = new List<float>();

    // 股票信息
    private EStockType stockType;
    public EStockType StockType => stockType;
    private string stockName;
    private Color themeColor;

    public float ViewPrice => prices.Count > 0 ? prices[prices.Count - 1] : 0;

    void Start()
    {
        InitialBackground();
        InitializeStockDisplay();
        // OptimizeForScreenResolution();
    }

    /// <summary>
    /// 根据屏幕分辨率优化显示参数
    /// </summary>
    private void OptimizeForScreenResolution()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 针对1920x1080屏幕的特殊优化
        if (screenWidth == 1920 && screenHeight == 1080)
        {
            // 三分屏布局优化
            chartWidth = 7.5f;   // 适合三等分布局
            chartHeight = 4.5f;  // 保持合理比例
            padding = 0.1f;      // 最小化边距
            pointSize = 0.12f;   // 更小的点
            maxPoints = 25;      // 减少点数避免拥挤

            if (showDebugInfo)
            {
                Debug.Log($"LineView已针对{screenWidth}x{screenHeight}分辨率优化");
            }
        }
        // 针对其他常见分辨率的优化
        else if (screenWidth >= 1600)
        {
            // 高分辨率屏幕
            chartWidth = 8f;
            chartHeight = 5f;
            maxPoints = 35;
        }
        else if (screenWidth >= 1366)
        {
            // 标准笔记本分辨率
            chartWidth = 6f;
            chartHeight = 4f;
            maxPoints = 20;
        }

        // 应用LineRenderer的宽度设置
        if (kLineRenderer != null)
        {
            kLineRenderer.startWidth = lineWidth;
            kLineRenderer.endWidth = lineWidth;
        }
    }

    [Header("调试设置")]
    [SerializeField] private bool showDebugInfo = false;

    /// <summary>
    /// 初始化股票信息显示
    /// </summary>
    private void InitializeStockDisplay()
    {
        if (titleText != null)
        {
            titleText.text = stockName ?? "股票";
        }

        // 设置主题颜色
        if (kLineRenderer != null)
        {
            Color targetColor = themeColor != Color.clear ? themeColor : lineColor;
            kLineRenderer.startColor = targetColor;
            kLineRenderer.endColor = targetColor;
        }
    }

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
        if (currentPriceText != null)
        {
            currentPriceText.text = $"¥{currentPrice:F2}";
        }

        if (changePercentText != null)
        {
            string changeSymbol = changePercent > 0 ? "+" : "";
            changePercentText.text = $"{changeSymbol}{changePercent:F1}%";

            // 设置颜色
            Color textColor = changePercent > 0 ? positiveColor :
                             changePercent < 0 ? negativeColor : neutralColor;
            changePercentText.color = textColor;
        }
    }

    void InitialBackground()
    {

        // 添加SpriteRenderer
        SpriteRenderer bgRenderer = background.GetComponent<SpriteRenderer>();

        // 创建Sprite
        Sprite sprite = bgRenderer.sprite;

        // 调整大小
        float scaleX = chartWidth / sprite.bounds.size.x;
        float scaleY = chartHeight / sprite.bounds.size.y;
        background.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    public void UpdateChart()
    {
        if (prices.Count == 0) return;

        // 清除现有点
        foreach (var point in points)
            if (point != null) DestroyImmediate(point);
        points.Clear();

        // 计算价格范围
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
            // 如果只有一个数据点，设置一个合理的价格范围
            minPrice = prices[0] - 1f;
            maxPrice = prices[0] + 1f;
        }
        else if (Mathf.Approximately(minPrice, maxPrice))
        {
            // 如果所有价格都相同，设置一个合理的范围
            minPrice -= 1f;
            maxPrice += 1f;
        }

        // 添加一些边距
        float priceRange = maxPrice - minPrice;
        if (priceRange < 1f) priceRange = 1f;
        minPrice -= priceRange * 0.1f;
        maxPrice += priceRange * 0.1f;

        // 更新线条
        kLineRenderer.positionCount = prices.Count;
        Vector3[] positions = new Vector3[prices.Count];

        for (int i = 0; i < prices.Count; i++)
        {
            // 安全计算X坐标
            float normalizedX;
            if (prices.Count == 1)
            {
                normalizedX = 0.0f; // 单个数据点居左显示
            }
            else
            {
                normalizedX = (float)i / (prices.Count - 1);
            }

            float x = Mathf.Lerp(-chartWidth / 2 + padding, chartWidth / 2 - padding, normalizedX);

            // 安全计算Y坐标
            float normalizedY = Mathf.InverseLerp(minPrice, maxPrice, prices[i]);
            float y = Mathf.Lerp(-chartHeight / 2 + padding, chartHeight / 2 - padding, normalizedY);

            // 验证坐标值
            if (float.IsNaN(x) || float.IsNaN(y))
            {
                Debug.LogWarning($"检测到NaN坐标值，跳过点 {i}。价格: {prices[i]}, X: {x}, Y: {y}");
                continue;
            }

            positions[i] = new Vector3(x, y, 0);

            // 创建点
            CreatePoint(positions[i], prices[i], i);
        }

        kLineRenderer.SetPositions(positions);
    }

    void CreatePoint(Vector3 position, float price, int index)
    {
        // 验证位置参数
        if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z))
        {
            Debug.LogError($"尝试创建无效位置的点 {index}，位置: {position}");
            return;
        }

        GameObject pointObj = Instantiate(pointPrefab, transform);

        Point point = pointObj.GetComponent<Point>();

        // 判断是否默认显示价格标签：每五个显示一个，其他悬停显示
        bool isDefaultVisible = (index == prices.Count - 1) || index == 0;
        point.RefreshPriceLabel(price, index, isDefaultVisible);

        point.name = $"Point_{index}";
        point.transform.SetParent(transform);
        point.transform.localPosition = position;

        // 设置点的颜色和大小
        point.SetColor(pointColor);
        point.SetSize(pointSize);

        points.Add(pointObj);
    }

    // 公共方法
    public void SetNewData(List<float> newPrices)
    {
        if (newPrices == null)
        {
            Debug.LogError("传入的价格数据为null");
            return;
        }

        // 验证所有价格数据
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
            return;
        }

        // 如果数据超过最大点数，只保留最新的maxPoints个点
        if (validPrices.Count > maxPoints)
        {
            int startIndex = validPrices.Count - maxPoints;
            validPrices = validPrices.GetRange(startIndex, maxPoints);
        }

        prices = validPrices;
        UpdateChart();
    }
    public void SetNewPrice(float newPrice)
    {
        prices.Add(newPrice);

        // 如果超过最大点数，移除最早的点
        if (prices.Count > maxPoints)
        {
            prices.RemoveAt(0);
        }

        UpdateChart();
    }

    public void ClearChart()
    {
        prices.Clear();
        UpdateChart();
    }
}