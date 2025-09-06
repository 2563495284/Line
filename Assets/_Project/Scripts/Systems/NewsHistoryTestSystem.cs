using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 新闻历史记录系统测试脚本
/// 用于测试各种新闻类型和历史记录功能
/// </summary>
public class NewsHistoryTestSystem : MonoBehaviour
{
    [Header("测试按钮")]
    [SerializeField] private Button testInfoButton;
    [SerializeField] private Button testCardPlayButton;
    [SerializeField] private Button testTradeButton;
    [SerializeField] private Button testPositiveButton;
    [SerializeField] private Button testNegativeButton;
    [SerializeField] private Button testMarketEventButton;
    [SerializeField] private Button testPredictionButton;

    [Header("批量测试")]
    [SerializeField] private Button batchTestButton;
    [SerializeField] private Button clearHistoryButton;
    [SerializeField] private Button forceMergeButton;

    [Header("测试设置")]
    [SerializeField] private int batchTestCount = 10; // 批量测试数量

    private void Start()
    {
        SetupTestButtons();
    }

    /// <summary>
    /// 设置测试按钮
    /// </summary>
    private void SetupTestButtons()
    {
        // 测试各种新闻类型
        if (testInfoButton != null)
            testInfoButton.onClick.AddListener(() => TestNewsType(NewsType.Info));

        if (testCardPlayButton != null)
            testCardPlayButton.onClick.AddListener(() => TestNewsType(NewsType.CardPlay));

        if (testTradeButton != null)
            testTradeButton.onClick.AddListener(() => TestNewsType(NewsType.Trade));

        if (testPositiveButton != null)
            testPositiveButton.onClick.AddListener(() => TestNewsType(NewsType.Positive));

        if (testNegativeButton != null)
            testNegativeButton.onClick.AddListener(() => TestNewsType(NewsType.Negative));

        if (testMarketEventButton != null)
            testMarketEventButton.onClick.AddListener(() => TestNewsType(NewsType.MarketEvent));

        if (testPredictionButton != null)
            testPredictionButton.onClick.AddListener(() => TestNewsType(NewsType.Prediction));

        // 批量测试
        if (batchTestButton != null)
            batchTestButton.onClick.AddListener(BatchTest);

        if (clearHistoryButton != null)
            clearHistoryButton.onClick.AddListener(ClearHistory);

        if (forceMergeButton != null)
            forceMergeButton.onClick.AddListener(ForceMerge);
    }

    /// <summary>
    /// 测试特定新闻类型
    /// </summary>
    private void TestNewsType(NewsType newsType)
    {
        string title = GetTitleByType(newsType);
        string content = GetContentByType(newsType);

        if (NewsSystem.Instance != null)
        {
            NewsSystem.Instance.BroadcastNews(title, content, newsType, 5f);
            Debug.Log($"测试新闻类型: {newsType} - {title}");
        }
        else
        {
            Debug.LogWarning("NewsSystem实例不存在");
        }
    }

    /// <summary>
    /// 根据新闻类型获取标题
    /// </summary>
    private string GetTitleByType(NewsType newsType)
    {
        switch (newsType)
        {
            case NewsType.Info:
                return "📰 信息通知";
            case NewsType.CardPlay:
                return "🎴 卡牌出牌";
            case NewsType.Trade:
                return "💰 交易动态";
            case NewsType.Positive:
                return "📈 正面消息";
            case NewsType.Negative:
                return "📉 负面消息";
            case NewsType.MarketEvent:
                return "🏛️ 市场事件";
            case NewsType.Prediction:
                return "🔮 预测信息";
            default:
                return "📰 测试新闻";
        }
    }

    /// <summary>
    /// 根据新闻类型获取内容
    /// </summary>
    private string GetContentByType(NewsType newsType)
    {
        switch (newsType)
        {
            case NewsType.Info:
                return "这是一条测试信息通知，用于验证新闻历史记录系统的功能。";
            case NewsType.CardPlay:
                return "玩家打出了一张测试卡牌，效果是增加10点攻击力。";
            case NewsType.Trade:
                return "玩家购买了100股测试股票，价格为15.50元。";
            case NewsType.Positive:
                return "市场表现良好，股票价格上涨了5%，投资者信心增强。";
            case NewsType.Negative:
                return "市场出现波动，股票价格下跌了3%，投资者需要谨慎。";
            case NewsType.MarketEvent:
                return "重要市场事件发生，可能影响相关股票的价格走势。";
            case NewsType.Prediction:
                return "分析师预测下周市场将保持稳定，建议投资者关注。";
            default:
                return "这是一条测试新闻内容，用于验证系统功能。";
        }
    }

    /// <summary>
    /// 批量测试
    /// </summary>
    private void BatchTest()
    {
        if (NewsSystem.Instance == null)
        {
            Debug.LogWarning("NewsSystem实例不存在");
            return;
        }

        Debug.Log($"开始批量测试，将发送 {batchTestCount} 条测试新闻");

        for (int i = 0; i < batchTestCount; i++)
        {
            NewsType randomType = GetRandomNewsType();
            string title = $"批量测试 {i + 1} - {GetTitleByType(randomType)}";
            string content = $"这是第 {i + 1} 条批量测试新闻，类型为 {randomType}。";

            // 延迟发送，避免同时发送太多
            float delay = i * 0.5f;
            StartCoroutine(DelayedNews(delay, title, content, randomType));
        }
    }

    /// <summary>
    /// 延迟发送新闻
    /// </summary>
    private System.Collections.IEnumerator DelayedNews(float delay, string title, string content, NewsType newsType)
    {
        yield return new WaitForSeconds(delay);
        NewsSystem.Instance.BroadcastNews(title, content, newsType, 5f);
    }

    /// <summary>
    /// 获取随机新闻类型
    /// </summary>
    private NewsType GetRandomNewsType()
    {
        NewsType[] types = {
            NewsType.Info,
            NewsType.CardPlay,
            NewsType.Trade,
            NewsType.Positive,
            NewsType.Negative,
            NewsType.MarketEvent,
            NewsType.Prediction
        };

        return types[Random.Range(0, types.Length)];
    }

    /// <summary>
    /// 清除历史记录
    /// </summary>
    private void ClearHistory()
    {
        if (NewsHistorySystem.Instance != null)
        {
            NewsHistorySystem.Instance.ClearAllHistory();
            Debug.Log("已清除所有历史记录");
        }
        else
        {
            Debug.LogWarning("NewsHistorySystem实例不存在");
        }
    }

    /// <summary>
    /// 强制合并
    /// </summary>
    private void ForceMerge()
    {
        if (NewsHistorySystem.Instance != null)
        {
            NewsHistorySystem.Instance.ForceMerge();
            Debug.Log("已强制触发新闻合并");
        }
        else
        {
            Debug.LogWarning("NewsHistorySystem实例不存在");
        }
    }

    /// <summary>
    /// 显示系统状态
    /// </summary>
    private void OnGUI()
    {
        if (NewsHistorySystem.Instance != null)
        {
            GUI.Label(new Rect(10, 10, 400, 100), $"新闻历史系统状态:\n{NewsHistorySystem.Instance.GetAllHistory().Count}");
        }

        if (NewsSystem.Instance != null)
        {
            string newsStatus = NewsSystem.Instance.GetStatusInfo();
            GUI.Label(new Rect(10, 120, 400, 100), $"新闻系统状态:\n{newsStatus}");
        }
    }

    /// <summary>
    /// 键盘快捷键
    /// </summary>
    private void Update()
    {
        // 数字键1-7测试不同新闻类型
        if (Input.GetKeyDown(KeyCode.Alpha1))
            TestNewsType(NewsType.Info);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            TestNewsType(NewsType.CardPlay);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            TestNewsType(NewsType.Trade);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            TestNewsType(NewsType.Positive);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            TestNewsType(NewsType.Negative);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            TestNewsType(NewsType.MarketEvent);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            TestNewsType(NewsType.Prediction);

        // B键批量测试
        if (Input.GetKeyDown(KeyCode.B))
            BatchTest();

        // C键清除历史
        if (Input.GetKeyDown(KeyCode.C))
            ClearHistory();

        // M键强制合并
        if (Input.GetKeyDown(KeyCode.M))
            ForceMerge();
    }
}
