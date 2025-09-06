using UnityEngine;
using TMPro;

/// <summary>
/// 三K线显示系统创建器
/// </summary>
public class TripleKLineCreator : MonoBehaviour
{
    [Header("预制体引用")]
    [SerializeField] private GameObject lineViewPrefab;
    [SerializeField] private GameObject textPrefab; // TextMeshPro文本预制体

    [Header("创建设置")]
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Vector3 centerPosition = Vector3.zero;
    [SerializeField] private float spacing = 12f; // 针对1920x1080优化的间距

    [Header("K线图设置")]
    [SerializeField] private float chartWidth = 7.5f;  // 适合1920x1080三分屏
    [SerializeField] private float chartHeight = 4.5f; // 保持合理比例

    /// <summary>
    /// 针对1920x1080分辨率的优化配置
    /// </summary>
    [ContextMenu("优化为1920x1080三分屏")]
    public void OptimizeFor1920x1080()
    {
        spacing = 12f;
        centerPosition = Vector3.zero;
        chartWidth = 7.5f;
        chartHeight = 4.5f;

        Debug.Log("已优化为1920x1080三分屏布局");
    }
    /// <summary>
    /// 创建三K线显示系统
    /// </summary>
    [ContextMenu("创建三K线显示系统")]
    public TripleKLineDisplay CreateTripleKLineSystem()
    {
        if (lineViewPrefab == null)
        {
            Debug.LogError("LineView预制体未设置！");
            return null;
        }

        if (parentTransform == null)
        {
            parentTransform = transform;
        }

        // 创建主容器
        GameObject mainContainer = new GameObject("TripleKLineDisplay");
        mainContainer.transform.SetParent(parentTransform);
        mainContainer.transform.position = centerPosition;

        // 添加TripleKLineDisplay组件
        TripleKLineDisplay tripleDisplay = mainContainer.AddComponent<TripleKLineDisplay>();

        // 创建三个LineView
        LineView oilLineView = CreateSingleLineView("Oil_LineView", EStockType.Oil,
            centerPosition + Vector3.left * spacing, mainContainer.transform);
        LineView steelLineView = CreateSingleLineView("Steel_LineView", EStockType.Steel,
            centerPosition, mainContainer.transform);
        LineView cottonLineView = CreateSingleLineView("Cotton_LineView", EStockType.Cotton,
            centerPosition + Vector3.right * spacing, mainContainer.transform);

        // 设置TripleKLineDisplay的引用
        SetTripleDisplayReferences(tripleDisplay, oilLineView, steelLineView, cottonLineView);

        Debug.Log("三K线显示系统创建完成！");
        return tripleDisplay;
    }

    /// <summary>
    /// 创建单个LineView
    /// </summary>
    private LineView CreateSingleLineView(string name, EStockType stockType, Vector3 position, Transform parent)
    {
        GameObject lineViewObj = Instantiate(lineViewPrefab, parent);
        lineViewObj.name = name;
        lineViewObj.transform.position = position;

        LineView lineView = lineViewObj.GetComponent<LineView>();
        if (lineView == null)
        {
            lineView = lineViewObj.AddComponent<LineView>();
        }

        // 创建标题和价格显示文本
        CreateTextElements(lineViewObj, stockType);

        // 设置股票信息
        var stockData = GetStockData(stockType);
        lineView.SetStockInfo(stockType, stockData.name, stockData.color);

        return lineView;
    }

    /// <summary>
    /// 创建文本元素
    /// </summary>
    private void CreateTextElements(GameObject lineViewObj, EStockType stockType)
    {
        // 创建标题文本
        GameObject titleObj = CreateTextObject("Title", lineViewObj.transform,
            new Vector3(0, chartHeight / 2 + 1f, 0), 24f);

        // 创建当前价格文本
        GameObject priceObj = CreateTextObject("CurrentPrice", lineViewObj.transform,
            new Vector3(-chartWidth / 4, chartHeight / 2 + 0.5f, 0), 18f);

        // 创建变化百分比文本
        GameObject changeObj = CreateTextObject("ChangePercent", lineViewObj.transform,
            new Vector3(chartWidth / 4, chartHeight / 2 + 0.5f, 0), 16f);

        // 设置标题文本内容
        var stockData = GetStockData(stockType);
        titleObj.GetComponent<TextMeshProUGUI>().text = stockData.name;
        titleObj.GetComponent<TextMeshProUGUI>().color = stockData.color;
    }

    /// <summary>
    /// 创建文本对象
    /// </summary>
    private GameObject CreateTextObject(string name, Transform parent, Vector3 localPosition, float fontSize)
    {
        GameObject textObj;

        if (textPrefab != null)
        {
            textObj = Instantiate(textPrefab, parent);
        }
        else
        {
            // 如果没有预制体，创建基础TextMeshPro对象
            textObj = new GameObject(name);
            textObj.transform.SetParent(parent);
            textObj.AddComponent<TextMeshProUGUI>();
        }

        textObj.name = name;
        textObj.transform.localPosition = localPosition;

        TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.fontSize = fontSize;
            textComponent.alignment = TextAlignmentOptions.Center;
        }

        return textObj;
    }

    /// <summary>
    /// 设置TripleKLineDisplay的引用
    /// </summary>
    private void SetTripleDisplayReferences(TripleKLineDisplay tripleDisplay,
        LineView oilView, LineView steelView, LineView cottonView)
    {
        tripleDisplay.SetLineViewReferences(oilView, steelView, cottonView);
        Debug.Log("三K线显示引用设置完成");
    }

    /// <summary>
    /// 获取股票数据
    /// </summary>
    private (string name, Color color) GetStockData(EStockType stockType)
    {
        switch (stockType)
        {
            case EStockType.Oil:
                return ("石油", new Color(0.2f, 0.2f, 0.2f));
            case EStockType.Steel:
                return ("钢铁", new Color(0.7f, 0.7f, 0.7f));
            case EStockType.Cotton:
                return ("棉花", new Color(1));
            default:
                return ("未知", Color.white);
        }
    }

    /// <summary>
    /// 在场景中查找现有的LineView并组装
    /// </summary>
    [ContextMenu("组装现有LineView")]
    public void AssembleExistingLineViews()
    {
        LineView[] existingLineViews = FindObjectsOfType<LineView>();

        if (existingLineViews.Length < 3)
        {
            Debug.LogWarning("场景中的LineView少于3个，无法组装三K线系统");
            return;
        }

        // 创建TripleKLineDisplay容器
        GameObject container = new GameObject("TripleKLineDisplay");
        container.transform.position = centerPosition;

        TripleKLineDisplay tripleDisplay = container.AddComponent<TripleKLineDisplay>();

        // 分配前三个LineView
        if (existingLineViews.Length >= 3)
        {
            existingLineViews[0].SetStockInfo(EStockType.Oil, "石油", new Color(1f, 1f, 1f));
            existingLineViews[1].SetStockInfo(EStockType.Steel, "钢铁", new Color(1f, 1f, 1f));
            existingLineViews[2].SetStockInfo(EStockType.Cotton, "棉花", new Color(1f, 1f, 1f));

            tripleDisplay.SetLineViewReferences(existingLineViews[0], existingLineViews[1], existingLineViews[2]);

            Debug.Log("现有LineView组装完成！");
        }
    }
}
