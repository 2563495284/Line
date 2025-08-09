using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TipsSystem : Singleton<TipsSystem>
{
    [Header("提示设置")]
    [SerializeField] private TipsUI tipsUIPrefab;
    [SerializeField] private Transform tipsParent;
    [SerializeField] private int maxTipsOnScreen = 3; // 最大同时显示的提示数量

    [Header("默认动画设置")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float moveDistance = 50f; // 向上移动的距离

    private Queue<TipsUI> activeTips = new Queue<TipsUI>();
    private Queue<TipsUI> tipsPool = new Queue<TipsUI>();

    protected override void Awake()
    {
        base.Awake();

        // 如果没有指定父对象，使用Canvas
        if (tipsParent == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                tipsParent = canvas.transform;
            }
        }
    }

    /// <summary>
    /// 显示提示消息
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="tipsType">提示类型</param>
    /// <param name="duration">显示时长</param>
    public void ShowTip(string message, TipsType tipsType = TipsType.Info, float duration = -1)
    {
        if (duration < 0) duration = displayDuration;

        TipsUI tipsUI = GetOrCreateTipsUI();
        tipsUI.ShowTip(message, tipsType, duration, fadeInDuration, fadeOutDuration, moveDistance);

        // 添加到活动队列
        activeTips.Enqueue(tipsUI);

        // 限制同时显示的提示数量
        if (activeTips.Count > maxTipsOnScreen)
        {
            TipsUI oldTip = activeTips.Dequeue();
            oldTip.HideTip();
        }
    }

    /// <summary>
    /// 显示成功提示
    /// </summary>
    public void ShowSuccess(string message, float duration = -1)
    {
        ShowTip(message, TipsType.Success, duration);
    }

    /// <summary>
    /// 显示错误提示
    /// </summary>
    public void ShowError(string message, float duration = -1)
    {
        ShowTip(message, TipsType.Error, duration);
    }

    /// <summary>
    /// 显示警告提示
    /// </summary>
    public void ShowWarning(string message, float duration = -1)
    {
        ShowTip(message, TipsType.Warning, duration);
    }

    /// <summary>
    /// 显示信息提示
    /// </summary>
    public void ShowInfo(string message, float duration = -1)
    {
        ShowTip(message, TipsType.Info, duration);
    }

    /// <summary>
    /// 获取或创建TipsUI
    /// </summary>
    private TipsUI GetOrCreateTipsUI()
    {
        TipsUI tipsUI;

        if (tipsPool.Count > 0)
        {
            tipsUI = tipsPool.Dequeue();
            tipsUI.gameObject.SetActive(true);
        }
        else
        {
            tipsUI = Instantiate(tipsUIPrefab, tipsParent);
        }

        return tipsUI;
    }

    /// <summary>
    /// 回收TipsUI到对象池
    /// </summary>
    public void RecycleTipsUI(TipsUI tipsUI)
    {
        tipsUI.gameObject.SetActive(false);
        tipsPool.Enqueue(tipsUI);
    }

    /// <summary>
    /// 清除所有提示
    /// </summary>
    public void ClearAllTips()
    {
        while (activeTips.Count > 0)
        {
            TipsUI tip = activeTips.Dequeue();
            tip.HideTip();
        }
    }
}

/// <summary>
/// 提示类型
/// </summary>
public enum TipsType
{
    Info,       // 信息
    Success,    // 成功
    Warning,    // 警告
    Error       // 错误
}