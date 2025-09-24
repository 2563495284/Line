using System.Collections.Generic;
using UnityEngine;

public class TipsView : LevelView
{

    [Header("提示设置")]
    [SerializeField] private GameObject tipsPrefab;
    [SerializeField] private int maxTipsOnScreen = 3; // 最大同时显示的提示数量

    [Header("默认动画设置")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float moveDistance = 50f; // 向上移动的距离

    private Queue<TipsCom> activeTips = new Queue<TipsCom>();
    protected override void OnShow()
    {
        Register<PopupTipsArgs>(NotifyConst.PopupTips, OnPopupTips);
    }
    protected override void OnHide()
    {
        Unregister<PopupTipsArgs>(NotifyConst.PopupTips, OnPopupTips);
        tipsPrefab.OPClear();
    }
    /// <summary>
    /// 显示提示消息
    /// </summary>
    public void OnPopupTips(PopupTipsArgs args)
    {
        if (args.duration < 0) args.duration = displayDuration;
        TipsCom tipsUI = tipsPrefab.OPGet().GetComponent<TipsCom>();
        AddCanvasCom(tipsUI.gameObject);
        tipsUI.ShowTip(args.message, args.tipsType, args.duration, fadeInDuration, fadeOutDuration, moveDistance);

        // 添加到活动队列
        activeTips.Enqueue(tipsUI);

        // 限制同时显示的提示数量
        if (activeTips.Count > maxTipsOnScreen)
        {
            TipsCom oldTip = activeTips.Dequeue();
            oldTip.HideTip();
        }
    }
}