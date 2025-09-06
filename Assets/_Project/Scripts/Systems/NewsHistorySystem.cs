using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 新闻历史记录系统
/// 管理新闻的历史记录，支持新闻合并和滑动浏览
/// </summary>
public class NewsHistorySystem : Singleton<NewsHistorySystem>
{
    [Header("历史记录设置")]
    [SerializeField] private int maxHistoryCount = 10; // 最大历史记录数量

    [Header("UI引用")]
    [SerializeField] private NewsHistoryUI historyUI;

    // 新闻历史记录列表（最新的在最后）
    private List<NewsHistoryItem> newsHistory = new List<NewsHistoryItem>();

    // 当前合并组
    private List<NewsHistoryItem> currentMergeGroup = new List<NewsHistoryItem>();

    public event Action<List<NewsHistoryItem>> OnHistoryUpdated;
    public event Action OnMergeGroupCompleted;

    protected override void Awake()
    {
        base.Awake();

        Debug.Log("NewsHistorySystem Awake被调用");

        // 如果没有指定UI，尝试自动查找
        if (historyUI == null)
        {
            historyUI = FindObjectOfType<NewsHistoryUI>();
            if (historyUI != null)
            {
                Debug.Log("自动找到NewsHistoryUI");
            }
            else
            {
                Debug.LogWarning("未找到NewsHistoryUI");
            }
        }
        else
        {
            Debug.Log("NewsHistoryUI引用已设置");
        }

        Debug.Log($"NewsHistorySystem初始化完成，实例ID: {GetInstanceID()}");
    }

    /// <summary>
    /// 添加新闻到历史记录
    /// </summary>
    public void AddNewsToHistory(string title, string content, NewsType newsType, float timestamp)
    {
        Debug.Log($"开始添加新闻到历史记录: {title}");

        var newsItem = new NewsHistoryItem
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
        Debug.Log($"新闻已添加到历史记录列表，当前数量: {newsHistory.Count}");

        // 限制历史记录数量
        if (newsHistory.Count > maxHistoryCount)
        {
            newsHistory.RemoveAt(0);
            Debug.Log("已达到最大历史记录数量，移除最旧的记录");
        }

        // 添加到当前合并组
        currentMergeGroup.Add(newsItem);
        Debug.Log($"新闻已添加到合并组，当前合并组数量: {currentMergeGroup.Count}");

        // 通知UI更新
        if (OnHistoryUpdated != null)
        {
            OnHistoryUpdated.Invoke(GetVisibleHistory());
            Debug.Log("已通知UI更新历史记录");
        }
        else
        {
            Debug.LogWarning("OnHistoryUpdated事件没有订阅者!");
        }

        Debug.Log($"添加新闻到历史记录完成: {title} (当前历史记录: {newsHistory.Count}, 合并组: {currentMergeGroup.Count})");
    }

    /// <summary>
    /// 获取可见的历史记录（未合并的）
    /// </summary>
    public List<NewsHistoryItem> GetVisibleHistory()
    {
        return newsHistory.Where(item => !item.isMerged).ToList();
    }

    /// <summary>
    /// 获取所有历史记录
    /// </summary>
    public List<NewsHistoryItem> GetAllHistory()
    {
        return new List<NewsHistoryItem>(newsHistory);
    }

    /// <summary>
    /// 清除已合并的新闻
    /// </summary>
    public void ClearMergedNews()
    {
        newsHistory.RemoveAll(item => item.isMerged);
        OnHistoryUpdated?.Invoke(GetVisibleHistory());
        Debug.Log($"清除已合并新闻，剩余历史记录: {newsHistory.Count}");
    }

    /// <summary>
    /// 清除所有历史记录
    /// </summary>
    public void ClearAllHistory()
    {
        newsHistory.Clear();
        currentMergeGroup.Clear();
        OnHistoryUpdated?.Invoke(GetVisibleHistory());
        Debug.Log("已清除所有历史记录");
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
            Debug.Log("强制合并完成");
        }
    }
}

/// <summary>
/// 新闻历史记录项
/// </summary>
[System.Serializable]
public class NewsHistoryItem
{
    public string id;
    public string title;
    public string content;
    public NewsType newsType;
    public float timestamp;
    public bool isMerged;

    public string GetTimeString()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(Time.time - timestamp);
        if (timeSpan.TotalMinutes < 1)
        {
            return "刚刚";
        }
        else if (timeSpan.TotalMinutes < 60)
        {
            return $"{(int)timeSpan.TotalMinutes}分钟前";
        }
        else if (timeSpan.TotalHours < 24)
        {
            return $"{(int)timeSpan.TotalHours}小时前";
        }
        else
        {
            return $"{(int)timeSpan.TotalDays}天前";
        }
    }
}
