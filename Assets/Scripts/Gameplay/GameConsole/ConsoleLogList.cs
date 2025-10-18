using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
//所有ConsoleLogItem居上对齐锚点一致在左上角，往下伸展
class LogRenderInfo
{
    public LogData data;
    public ConsoleLogItem item;
    public Color? specColor;
    public float height;
    public int index;
    public float posY;
    public float Top => posY;
    public float Bottom => posY - height;
}
/// <summary>
/// 虚拟日志列表（适配TMPLogItem）
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class ConsoleLogList : MonoBehaviour
{
    [Header("列表配置")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewPort;
    [SerializeField] private RectTransform content;  // 内容容器
    [SerializeField] private GameObject logItemPrefab; //日志项预制体

    [Header("布局参数")]
    [SerializeField] private float spacing = 4f; // Item间距
    [SerializeField] private float scrollSensitivity = 8f;
    [SerializeField] private float scrollSpeed = 5f;


    private Queue<LogRenderInfo> renderList = new Queue<LogRenderInfo>();
    private List<ConsoleLogItem> comList = new();
    public List<LogData> Logs => renderList.Select(e => e.data).ToList();
    private float contentHeight = 0;
    private float targetViewProgress = 1;
    private bool isInDragSlider = false;
    public void Init()
    {
        contentHeight = 0;
        scrollRect.onValueChanged.AddListener(OnScroll);
    }
    private void OnScroll(Vector2 v)
    {
        if (isInDragSlider)
            targetViewProgress = scrollRect.normalizedPosition.y;
        RefreshView();
    }

    private void Update()
    {
        // 获取鼠标滚轮输入（正负值表示上下滚动）
        float scrollInput = Input.mouseScrollDelta.y;
        // 如果设置了只在悬停时响应，检查鼠标是否在ScrollRect上
        if (IsMouseOverScrollRect() && !Mathf.Approximately(scrollInput, 0))
            HandleScroll(scrollInput);
        // 处理滚轮滚动
        Vector2 newPos = scrollRect.normalizedPosition;
        newPos.y = Mathf.Lerp(newPos.y, targetViewProgress, Mathf.Clamp01(scrollSpeed * Time.deltaTime));
        scrollRect.normalizedPosition = newPos;
    }

    /// <summary>
    /// 处理滚动逻辑
    /// </summary>
    private void HandleScroll(float scrollInput)
    {
        targetViewProgress = scrollRect.normalizedPosition.y;
        targetViewProgress += scrollInput * scrollSensitivity / (Input.GetKey(KeyCode.LeftControl) ? 50 : renderList.Count);
        targetViewProgress = Mathf.Clamp01(targetViewProgress);
    }
    public void OnStartDragSlider()
    {
        isInDragSlider = true;
    }
    public void OnReleaseSlider()
    {
        isInDragSlider = false;

    }

    /// <summary>
    /// 检查鼠标是否悬停在ScrollRect上
    /// </summary>
    private bool IsMouseOverScrollRect()
    {
        // 获取ScrollRect的RectTransform
        RectTransform rectTransform = scrollRect.GetComponent<RectTransform>();

        // 将鼠标位置转换为ScrollRect本地坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            scrollRect.GetComponentInParent<Canvas>().worldCamera,
            out Vector2 localPoint);

        // 检查本地坐标是否在ScrollRect的矩形范围内
        return rectTransform.rect.Contains(localPoint);
    }
    void OnEnable()
    {
        ApplyCache();
        RefreshContentHeight();
        ScrollToBottom();
        RefreshView();
    }
    void OnDisable()
    {
        comList.ForEach(e =>
        {
            e.OnRecycle();
            e.gameObject.OPPush();
        });
        comList.Clear();
        foreach (var e in renderList)
            e.item = null;
    }
    private List<(LogData data, Color? color)> preCache = new();
    public void AddLog(LogData logData, Color? specColor = null)
    {
        preCache.Add((logData, specColor));
        if (!gameObject.activeInHierarchy)
            return;
        ApplyCache();
        ScrollToBottom();
        RefreshView();
    }
    private void ApplyCache()
    {

        GameObject logItem = logItemPrefab.OPGet(content);
        ConsoleLogItem com = logItem.GetComponent<ConsoleLogItem>();
        preCache.ForEach(e =>
        {
            com.SetData(e.data, e.color);
            float height = com.Height;
            int idx = renderList.Count;
            float spacingOffset = renderList.Count > 0 ? spacing : 0;
            float posY = -contentHeight - spacingOffset;
            contentHeight = contentHeight + spacingOffset + height;
            if (renderList.Count == ConsoleConfig.maxConsoleNum)
                renderList.Dequeue();
            renderList.Enqueue(new LogRenderInfo
            {
                data = e.data,
                item = null,
                height = height,
                index = idx,
                posY = posY,
                specColor = e.color
            });
        });
        com.OnRecycle();
        com.gameObject.OPPush();
        preCache.Clear();
    }
    private void RefreshContentHeight()
    {
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);

    }
    private void RefreshView()
    {
        RefreshContentHeight();
        float viewHeight = viewPort.rect.height;
        bool isFullInsert = contentHeight < viewHeight;
        float viewTop = -content.anchoredPosition.y;
        float viewBottom = -content.anchoredPosition.y - viewHeight;
        foreach (LogRenderInfo info in renderList)
        {
            bool isVisible = isFullInsert || info.Top > viewBottom && info.Bottom < viewTop;
            if (isVisible && info.item == null)
            {
                ConsoleLogItem item = logItemPrefab.OPGet(content).GetComponent<ConsoleLogItem>();
                item.SetData(info.data, info.specColor);
                info.item = item;
                comList.Add(item);
            }
            else if (!isVisible && info.item != null)
            {
                comList.Remove(info.item);
                info.item.OnRecycle();
                info.item.gameObject.OPPush();
                info.item = null;
            }
            if (isVisible)
                info.item.SetAnchorPosY(info.posY - (isFullInsert ? viewHeight - contentHeight : 0));
        }
    }

    public void ClearLogs()
    {
        comList.ForEach(e =>
        {
            e.OnRecycle();
            e.gameObject.OPPush();
        });
        comList.Clear();
        renderList.Clear();
        contentHeight = 0;
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
    }

    public void ScrollToBottom()
    {
        targetViewProgress = 0;
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 0);
    }


}
