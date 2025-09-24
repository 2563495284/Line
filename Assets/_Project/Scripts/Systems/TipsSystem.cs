using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TipsSystem : SingletonCom<TipsSystem>
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

        Debug.Log($"显示提示: '{message}', 类型: {tipsType}, 时长: {duration}");
        Debug.Log($"显示前状态 - 活动队列: {activeTips.Count}, 对象池: {tipsPool.Count}");

        TipsUI tipsUI = GetOrCreateTipsUI();
        tipsUI.ShowTip(message, tipsType, duration, fadeInDuration, fadeOutDuration, moveDistance);

        // 添加到活动队列
        activeTips.Enqueue(tipsUI);
        Debug.Log($"已添加到活动队列，当前活动数量: {activeTips.Count}");

        // 限制同时显示的提示数量
        if (activeTips.Count > maxTipsOnScreen)
        {
            TipsUI oldTip = activeTips.Dequeue();
            Debug.Log($"超过最大显示数量，强制隐藏旧提示");
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
            Debug.Log($"从对象池获取TipsUI，剩余对象池数量: {tipsPool.Count}");
        }
        else
        {
            tipsUI = Instantiate(tipsUIPrefab, tipsParent);
            Debug.Log($"创建新的TipsUI实例");
        }

        return tipsUI;
    }

    /// <summary>
    /// 回收TipsUI到对象池
    /// </summary>
    public void RecycleTipsUI(TipsUI tipsUI)
    {
        // 从活动队列中移除（如果存在）
        RemoveFromActiveTips(tipsUI);

        // 重置TipsUI状态
        tipsUI.gameObject.SetActive(false);
        tipsPool.Enqueue(tipsUI);

        Debug.Log($"TipsUI已回收，活动队列数量: {activeTips.Count}, 对象池数量: {tipsPool.Count}");
    }

    /// <summary>
    /// 从活动队列中移除指定的TipsUI
    /// </summary>
    private void RemoveFromActiveTips(TipsUI tipsUI)
    {
        // 由于Queue不支持直接移除，需要重建队列
        Queue<TipsUI> tempQueue = new Queue<TipsUI>();

        while (activeTips.Count > 0)
        {
            TipsUI tip = activeTips.Dequeue();
            if (tip != tipsUI)
            {
                tempQueue.Enqueue(tip);
            }
        }

        // 将临时队列的内容放回原队列
        while (tempQueue.Count > 0)
        {
            activeTips.Enqueue(tempQueue.Dequeue());
        }
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
