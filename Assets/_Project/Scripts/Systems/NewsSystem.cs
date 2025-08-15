using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NewsSystem : Singleton<NewsSystem>
{
    private const int MAX_NEWS_ON_SCREEN = 5; // 最大同时显示的新闻数量

    [Header("新闻设置")]
    [SerializeField] private NewsUI newsUIPrefab;
    [SerializeField] private Transform newsParent;
    [SerializeField] private int maxNewsOnScreen = MAX_NEWS_ON_SCREEN; // 最大同时显示的新闻数量
    [SerializeField] private float newsSpacing = 10f; // 新闻之间的间距

    [Header("动画设置")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField]
    private SerializableDictionary<NewsType, float> displayDuration;
    [SerializeField] private float slideDistance = 100f; // 滑入距离

    private List<NewsUI> activeNews = new List<NewsUI>();
    private Queue<NewsUI> newsPool = new Queue<NewsUI>();

    protected override void Awake()
    {
        base.Awake();

        // 如果没有指定父对象，使用Canvas
        if (newsParent == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                newsParent = canvas.transform;
            }
        }
    }

    /// <summary>
    /// 播报新闻
    /// </summary>
    /// <param name="title">新闻标题</param>
    /// <param name="content">新闻内容</param>
    /// <param name="newsType">新闻类型</param>
    /// <param name="duration">显示时长</param>
    public void BroadcastNews(string title, string content, NewsType newsType = NewsType.Info, float duration = -1)
    {
        if (duration < 0) duration = displayDuration[newsType];

        // 检查是否需要移除旧新闻
        if (activeNews.Count >= maxNewsOnScreen)
        {
            NewsUI oldNews = activeNews[0];
            activeNews.RemoveAt(0);
            oldNews.HideNews();
        }

        NewsUI newsUI = GetOrCreateNewsUI();

        // 先添加到活动列表
        activeNews.Add(newsUI);

        // 重新排列所有新闻（设置目标位置）
        RearrangeNews();

        // 然后显示新闻
        newsUI.ShowNews(title, content, newsType, duration, fadeInDuration, fadeOutDuration, slideDistance);

        Debug.Log($"新闻播报: {title} - {content} (当前新闻数量: {activeNews.Count})");
    }
    /// <summary>
    /// 播报市场事件新闻
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="eventTitle">事件标题（可选，为空时使用默认标题）</param>
    /// <param name="customMessage">自定义消息（可选，为空时使用随机消息）</param>
    /// <param name="priceImpact">价格影响（可选）</param>
    public void BroadcastMarketEvent(EEventCardType eventType)
    {
        // 确定新闻标题
        string title = "新闻";

        // 获取消息内容
        string content = OilMarketMessages.GetRandomMessage(eventType);


        // 播报新闻
        BroadcastNews(title, content, NewsType.MarketEvent, displayDuration[NewsType.MarketEvent]);

        Debug.Log($"市场事件播报: {eventType} - {title}");
    }

    /// <summary>
    /// 播报NPC出牌新闻
    /// </summary>
    /// <param name="npcName">NPC名称</param>
    /// <param name="cardTitle">卡牌标题</param>
    /// <param name="effect">卡牌效果</param>
    public void BroadcastNPCCardPlay(string npcName, string cardTitle, string effect = "")
    {
        string title = $"📰 {npcName} 出牌";
        string content = $"打出卡牌: {cardTitle}";

        if (!string.IsNullOrEmpty(effect))
        {
            content += $"\n效果: {effect}";
        }

        BroadcastNews(title, content, NewsType.CardPlay);
    }

    /// <summary>
    /// 播报股票价格变化新闻
    /// </summary>
    /// <param name="oldPrice">旧价格</param>
    /// <param name="newPrice">新价格</param>
    /// <param name="reason">变化原因</param>
    public void BroadcastStockPriceChange(float oldPrice, float newPrice, string reason = "")
    {
        string changeType = newPrice > oldPrice ? "上涨" : "下跌";
        string title = $"石油价格{changeType}";
        float change = Mathf.Abs(newPrice - oldPrice);
        float changePercent = (change / oldPrice) * 100f;

        string content = $"{oldPrice:F2} → {newPrice:F2} ({changePercent:F1}%)";

        // if (!string.IsNullOrEmpty(reason))
        // {
        //     content += $"\n原因: {reason}";
        // }

        NewsType newsType = newPrice > oldPrice ? NewsType.Positive : NewsType.Negative;
        BroadcastNews(title, content, newsType);
    }

    /// <summary>
    /// 播报交易新闻
    /// </summary>
    /// <param name="playerName">玩家名称</param>
    /// <param name="action">交易动作</param>
    /// <param name="amount">数量</param>
    /// <param name="price">价格</param>
    public void BroadcastTrade(string playerName, string action, int amount, float price)
    {
        string title = "💰 交易动态";
        string content = $"{playerName} {action}了 {amount} 股股票\n价格: {price:F2}";

        BroadcastNews(title, content, NewsType.Trade);
    }
    /// <summary>
    /// 播报预测新闻
    /// </summary>
    /// <param name="playerName">玩家名称</param>
    /// <param name="action">交易动作</param>
    /// <param name="amount">数量</param>
    /// <param name="price">价格</param>
    public void BroadcastPrediction(string title, string content)
    {
        BroadcastNews(title, content, NewsType.Prediction);
    }

    /// <summary>
    /// 获取或创建NewsUI
    /// </summary>
    private NewsUI GetOrCreateNewsUI()
    {
        NewsUI newsUI;

        if (newsPool.Count > 0)
        {
            newsUI = newsPool.Dequeue();
            newsUI.gameObject.SetActive(true);
        }
        else
        {
            newsUI = Instantiate(newsUIPrefab, newsParent);
        }

        return newsUI;
    }

    /// <summary>
    /// 重新排列所有新闻
    /// </summary>
    private void RearrangeNews()
    {
        for (int i = 0; i < activeNews.Count; i++)
        {
            if (activeNews[i] != null)
            {
                Vector3 targetPosition = CalculateNewsPosition(i);
                activeNews[i].SetTargetPosition(targetPosition);
                Debug.Log($"新闻 {i} 位置设置为: {targetPosition}");
            }
        }
    }

    /// <summary>
    /// 计算新闻位置
    /// </summary>
    private Vector3 CalculateNewsPosition(int index)
    {
        // 获取新闻的实际高度
        float newsHeight = GetNewsHeight();

        // 从右上角开始，向下排列
        float yOffset = -index * (newsHeight + newsSpacing);
        return new Vector3(0, yOffset, 0);
    }

    /// <summary>
    /// 获取新闻高度
    /// </summary>
    private float GetNewsHeight()
    {
        if (newsUIPrefab != null)
        {
            RectTransform rectTransform = newsUIPrefab.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return rectTransform.rect.height;
            }
        }

        // 默认高度
        return 80f;
    }

    /// <summary>
    /// 回收NewsUI到对象池
    /// </summary>
    public void RecycleNewsUI(NewsUI newsUI)
    {
        if (activeNews.Remove(newsUI))
        {
            newsUI.gameObject.SetActive(false);
            newsPool.Enqueue(newsUI);
            RearrangeNews();
            Debug.Log($"回收新闻UI，当前活动新闻数量: {activeNews.Count}");
        }
    }

    /// <summary>
    /// 清除所有新闻
    /// </summary>
    public void ClearAllNews()
    {
        foreach (NewsUI news in activeNews)
        {
            if (news != null)
            {
                news.HideNews();
            }
        }
        activeNews.Clear();
        Debug.Log("已清除所有新闻");
    }

    /// <summary>
    /// 获取新闻系统状态信息
    /// </summary>
    public string GetStatusInfo()
    {
        return $"活动新闻: {activeNews.Count}/{maxNewsOnScreen}, 对象池: {newsPool.Count}";
    }

    /// <summary>
    /// 测试新闻系统
    /// </summary>
    public void TestNewsSystem()
    {
        BroadcastNews("测试新闻", "这是一个测试新闻", NewsType.Info, 3f);
        Debug.Log("测试新闻已发送");
    }

    /// <summary>
    /// 强制刷新所有新闻位置
    /// </summary>
    public void ForceRefreshPositions()
    {
        Debug.Log("强制刷新新闻位置");
        RearrangeNews();
    }
}

/// <summary>
/// 新闻类型
/// </summary>
public enum NewsType
{
    Info,       // 信息
    CardPlay,   // 出牌
    Trade,      // 交易
    Positive,   // 正面
    Negative,    // 负面
    MarketEvent,// 市场事件
    Prediction, // 预测
}