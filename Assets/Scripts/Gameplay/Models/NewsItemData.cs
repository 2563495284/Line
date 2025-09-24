
using System;
using UnityEngine;

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
[System.Serializable]
public class NewsItemData
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
