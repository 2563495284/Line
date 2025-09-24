using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NewsSystem : SingletonCom<NewsSystem>
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

        // 添加到历史记录系统
        if (NewsHistorySystem.Ins != null)
        {
            NewsHistorySystem.Ins.AddNewsToHistory(title, content, newsType, Time.time);
            Debug.Log($"新闻已添加到历史记录系统: {title}");
        }
        else
        {
            Debug.LogWarning("NewsHistorySystem实例不存在，无法添加新闻到历史记录!");
        }

        Debug.Log($"新闻播报: {title} - {content} (当前新闻数量: {activeNews.Count})");
    }

    /// <summary>
    /// 播报市场事件新闻（多股市版本，指定品类）
    /// </summary>
    public void BroadcastMarketEvent(EStockType stockType, EEventCardType eventType)
    {
        // 标题包含品类名
        string stockName = MultiStockSystem.Ins?.GetStockMarket(stockType)?.stockName ?? stockType.ToString();
        string title = $"新闻 - {stockName}";

        // 内容依据品类与事件类型
        string content = OilMarketMessages.GetRandomMessage(stockType, eventType);

        BroadcastNews(title, content, NewsType.MarketEvent, displayDuration[NewsType.MarketEvent]);
        Debug.Log($"市场事件播报: {stockType} {eventType} - {title}");
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
                return rectTransform.rect.height * rectTransform.localScale.y;
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
    /// 强制刷新所有新闻位置
    /// </summary>
    public void ForceRefreshPositions()
    {
        Debug.Log("强制刷新新闻位置");
        RearrangeNews();
    }
}
