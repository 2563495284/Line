using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 新闻历史记录系统
/// 管理新闻的历史记录，支持新闻合并和滑动浏览
/// </summary>
public class NewsHistorySystem : SingletonCom<NewsHistorySystem>
{
    [Header("历史记录设置")]
    [SerializeField] private int maxHistoryCount = 10; // 最大历史记录数量

    [Header("UI引用")]
    [SerializeField] private NewsHistoryUI historyUI;

    // 新闻历史记录列表（最新的在最后）
    private List<NewsItemData> newsHistory = new List<NewsItemData>();

    // 当前合并组
    private List<NewsItemData> currentMergeGroup = new List<NewsItemData>();

    public event Action<List<NewsItemData>> OnHistoryUpdated;
    public event Action OnMergeGroupCompleted;

    protected override void Awake()
    {
        base.Awake();


        // 如果没有指定UI，尝试自动查找
        if (historyUI == null)
        {
            historyUI = FindObjectOfType<NewsHistoryUI>();
            if (historyUI == null)
            {
                Debug.LogWarning("未找到NewsHistoryUI");
            }
        }

    }

    /// <summary>
    /// 添加新闻到历史记录
    /// </summary>
    public void AddNewsToHistory(string title, string content, NewsType newsType, float timestamp)
    {

        var newsItem = new NewsItemData
        {
            id = Guid.NewGuid().ToString(),
            title = title,
            content = content,
            newsType = newsType,
            timestamp = timestamp,
            isMerged = false
        };

        // 添加到历史记录
        newsHistory.Add(newsItem);

        // 限制历史记录数量
        if (newsHistory.Count > maxHistoryCount)
        {
            newsHistory.RemoveAt(0);
        }

        // 添加到当前合并组
        currentMergeGroup.Add(newsItem);

        // 通知UI更新
        if (OnHistoryUpdated != null)
        {
            OnHistoryUpdated.Invoke(GetVisibleHistory());
        }
        else
        {
            Debug.LogWarning("OnHistoryUpdated事件没有订阅者!");
        }

    }

    /// <summary>
    /// 获取可见的历史记录（未合并的）
    /// </summary>
    public List<NewsItemData> GetVisibleHistory()
    {
        return newsHistory.Where(item => !item.isMerged).ToList();
    }

    /// <summary>
    /// 获取所有历史记录
    /// </summary>
    public List<NewsItemData> GetAllHistory()
    {
        return new List<NewsItemData>(newsHistory);
    }

    /// <summary>
    /// 清除已合并的新闻
    /// </summary>
    public void ClearMergedNews()
    {
        newsHistory.RemoveAll(item => item.isMerged);
        OnHistoryUpdated?.Invoke(GetVisibleHistory());
    }

    /// <summary>
    /// 清除所有历史记录
    /// </summary>
    public void ClearAllHistory()
    {
        newsHistory.Clear();
        currentMergeGroup.Clear();
        OnHistoryUpdated?.Invoke(GetVisibleHistory());
    }

    /// <summary>
    /// 手动触发合并（用于测试）
    /// </summary>
    public void ForceMerge()
    {
        if (currentMergeGroup.Count > 0)
        {
            foreach (var item in currentMergeGroup)
            {
                item.isMerged = true;
            }
            currentMergeGroup.Clear();
            OnMergeGroupCompleted?.Invoke();
            OnHistoryUpdated?.Invoke(GetVisibleHistory());
        }
    }
}
