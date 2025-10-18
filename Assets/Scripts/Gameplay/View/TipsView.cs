using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TipsView : LevelView
{

    [Header("提示设置")]
    [SerializeField] private GameObject tipsPrefab;
    [SerializeField] private Transform folder;
    [SerializeField] private int maxTipsOnScreen = 3; // 最大同时显示的提示数量

    [Header("默认动画设置")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float moveDistance = 50f; // 向上移动的距离

    private Queue<TipsItem> activeTips = new Queue<TipsItem>();
    public override void OnEnter()
    {
        Register<PopupTipsArgs>(EventConst.PopupTips, OnPopupTips);
    }
    public override void OnExit()
    {
        Unregister<PopupTipsArgs>(EventConst.PopupTips, OnPopupTips);
        tipsPrefab.OPClear();
    }
    /// <summary>
    /// 显示提示消息
    /// </summary>
    private void OnPopupTips(PopupTipsArgs args)
    {
        if (args.duration < 0) args.duration = displayDuration;
        TipsItem tipsUI = tipsPrefab.OPGet(folder).GetComponent<TipsItem>();
        tipsUI.ShowTip(args.message, args.tipsType, args.duration, fadeInDuration, fadeOutDuration, moveDistance);

        // 添加到活动队列
        activeTips.Enqueue(tipsUI);

        // 限制同时显示的提示数量
        if (activeTips.Count > maxTipsOnScreen)
        {
            TipsItem oldTip = activeTips.Dequeue();
            oldTip.HideTip();
        }
    }
}