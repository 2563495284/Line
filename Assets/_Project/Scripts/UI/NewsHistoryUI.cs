using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

/// <summary>
/// 新闻历史记录UI组件
/// 显示在左下角，支持滑动浏览历史记录
/// </summary>
public class NewsHistoryUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform contentParent;
    [SerializeField] private NewsHistoryItemUI itemPrefab;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button expandButton;

    [Header("显示设置")]
    [SerializeField] private int maxVisibleItems = 8; // 最大可见项目数
    [SerializeField] private float itemHeight = 200f; // 每个项目的高度
    [SerializeField] private float itemSpacing = 5f; // 项目间距
    [SerializeField] private bool startExpanded = false; // 是否默认展开
    [SerializeField] private float headerHeight = 30f; // 按钮栏高度

    [Header("动画设置")]
    [SerializeField] private float expandDuration = 0.3f;
    [SerializeField] private float itemFadeInDuration = 0.2f;

    // UI状态
    private bool isExpanded = false;
    private List<NewsHistoryItemUI> itemUIs = new List<NewsHistoryItemUI>();
    private Vector2 collapsedSize;
    private Vector2 expandedSize;

    // 历史记录数据
    private List<NewsHistoryItem> currentHistory = new List<NewsHistoryItem>();

    private void Awake()
    {
        InitializeUI();
        SetupEventHandlers();

        // 设置初始状态
        isExpanded = startExpanded;
        UpdateUIState();
    }

    private void Start()
    {
        // 订阅历史记录更新事件
        if (NewsHistorySystem.Instance != null)
        {
            NewsHistorySystem.Instance.OnHistoryUpdated += OnHistoryUpdated;
            NewsHistorySystem.Instance.OnMergeGroupCompleted += OnMergeGroupCompleted;
        }

        // 初始化显示
        RefreshDisplay();
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (NewsHistorySystem.Instance != null)
        {
            NewsHistorySystem.Instance.OnHistoryUpdated -= OnHistoryUpdated;
            NewsHistorySystem.Instance.OnMergeGroupCompleted -= OnMergeGroupCompleted;
        }
    }

    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitializeUI()
    {
        // 如果没有指定ScrollRect，自动获取
        if (scrollRect == null)
        {
            scrollRect = GetComponentInChildren<ScrollRect>();
        }

        // 如果没有指定内容父对象，使用ScrollRect的content
        if (contentParent == null && scrollRect != null)
        {
            contentParent = scrollRect.content;
        }

        // 设置锚点到左下角
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1);
        rectTransform.anchorMax = new Vector2(0.5f, 1);
        rectTransform.pivot = new Vector2(0.5f, 1);

        // 设置初始尺寸 - 固定宽度，只改变高度
        collapsedSize = new Vector2(200f, 30f); // 只显示按钮区域高度
        expandedSize = new Vector2(200f, 470f); // 展开后显示完整内容

        rectTransform.sizeDelta = startExpanded ? expandedSize : collapsedSize;

        // 设置内容区域的初始位置
        if (contentParent != null)
        {
            RectTransform contentRect = contentParent.GetComponent<RectTransform>();
            if (contentRect != null)
            {
                float initialY = startExpanded ? -headerHeight : 0;
                contentRect.anchoredPosition = new Vector2(0, initialY);
                Debug.Log($"设置内容区域初始位置: {contentRect.anchoredPosition}");
            }
        }

        // 设置ScrollRect的初始尺寸
        if (scrollRect != null)
        {
            RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();
            if (scrollRectTransform != null)
            {
                Vector2 initialSize = startExpanded ?
                    new Vector2(200f, expandedSize.y - headerHeight) :
                    new Vector2(200f, 0);
                scrollRectTransform.sizeDelta = initialSize;
                Debug.Log($"设置ScrollRect初始尺寸: {scrollRectTransform.sizeDelta}");
            }
        }
    }

    /// <summary>
    /// 设置事件处理器
    /// </summary>
    private void SetupEventHandlers()
    {
        Debug.Log("开始设置事件处理器...");

        if (clearButton != null)
        {
            clearButton.onClick.AddListener(OnClearButtonClicked);
            Debug.Log("清除按钮事件已绑定");
        }
        else
        {
            Debug.LogWarning("清除按钮引用为空!");
        }

        if (expandButton != null)
        {
            expandButton.onClick.AddListener(OnExpandButtonClicked);
            Debug.Log("展开按钮事件已绑定");
        }
        else
        {
            Debug.LogWarning("展开按钮引用为空!");
        }

        Debug.Log("事件处理器设置完成");
    }

    /// <summary>
    /// 历史记录更新事件处理
    /// </summary>
    private void OnHistoryUpdated(List<NewsHistoryItem> history)
    {
        currentHistory = history;
        RefreshDisplay();
    }

    /// <summary>
    /// 合并组完成事件处理
    /// </summary>
    private void OnMergeGroupCompleted()
    {
        // 可以在这里添加合并完成的特效
        Debug.Log("新闻合并组完成，UI已更新");
    }

    /// <summary>
    /// 刷新显示
    /// </summary>
    private void RefreshDisplay()
    {
        // 清理旧的UI项目
        ClearItems();

        // 创建新的UI项目
        CreateItems();

        // 滚动到底部（显示最新消息）
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    /// <summary>
    /// 清理所有UI项目
    /// </summary>
    private void ClearItems()
    {
        foreach (var item in itemUIs)
        {
            if (item != null)
            {
                DestroyImmediate(item.gameObject);
            }
        }
        itemUIs.Clear();
    }

    /// <summary>
    /// 创建UI项目
    /// </summary>
    private void CreateItems()
    {
        if (contentParent == null || itemPrefab == null) return;

        // 从最新的新闻开始创建（倒序显示，最新的在底部）
        var reversedHistory = currentHistory.ToList();
        reversedHistory.Reverse();

        foreach (var historyItem in reversedHistory)
        {
            var itemUI = Instantiate(itemPrefab, contentParent);
            itemUI.Initialize(historyItem);
            itemUIs.Add(itemUI);

            // 设置位置
            int index = itemUIs.Count - 1;
            float yPos = -index * (itemHeight + itemSpacing);
            itemUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPos);

            // 确保高度设置正确
            var itemRectTransform = itemUI.GetComponent<RectTransform>();
            if (itemRectTransform != null)
            {
                itemRectTransform.sizeDelta = new Vector2(0, itemHeight);
            }

            // 淡入动画
            var canvasGroup = itemUI.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1f, itemFadeInDuration).SetDelay(index * 0.05f);
            }
        }

        // 更新内容区域高度
        UpdateContentHeight();
    }

    /// <summary>
    /// 更新内容区域高度
    /// </summary>
    private void UpdateContentHeight()
    {
        if (contentParent == null) return;

        float totalHeight = itemUIs.Count * (itemHeight + itemSpacing) - itemSpacing;
        contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, totalHeight);
    }

    /// <summary>
    /// 清除按钮点击事件
    /// </summary>
    private void OnClearButtonClicked()
    {
        if (NewsHistorySystem.Instance != null)
        {
            NewsHistorySystem.Instance.ClearAllHistory();
        }
    }

    /// <summary>
    /// 展开/收起按钮点击事件
    /// </summary>
    private void OnExpandButtonClicked()
    {
        Debug.Log($"展开按钮被点击! 当前状态: isExpanded={isExpanded}");
        isExpanded = !isExpanded;
        Debug.Log($"状态已切换为: isExpanded={isExpanded}");
        UpdateUIState();
    }

    /// <summary>
    /// 更新UI状态
    /// </summary>
    private void UpdateUIState()
    {
        Vector2 targetSize = isExpanded ? expandedSize : collapsedSize;

        // 只改变高度，保持宽度不变
        RectTransform rectTransform = GetComponent<RectTransform>();
        float currentWidth = rectTransform.sizeDelta.x;
        Vector2 newSize = new Vector2(currentWidth, targetSize.y);

        Debug.Log($"更新UI状态: 展开={isExpanded}, 当前尺寸={rectTransform.sizeDelta}, 目标尺寸={newSize}");

        // 动画改变高度
        rectTransform.DOSizeDelta(newSize, expandDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Debug.Log($"UI状态更新完成: 最终尺寸={rectTransform.sizeDelta}");
            });

        // 更新ScrollRect状态和尺寸
        if (scrollRect != null)
        {
            scrollRect.enabled = isExpanded;

            // 如果收起，滚动到顶部
            if (!isExpanded)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }

            // 调整ScrollRect的尺寸
            AdjustScrollRectSize();

            Debug.Log($"ScrollRect状态: 启用={scrollRect.enabled}, 垂直位置={scrollRect.verticalNormalizedPosition}");
        }

        // 调整内容区域位置
        AdjustContentAreaPosition();

        // 更新清除按钮状态
        if (clearButton != null)
        {
            clearButton.gameObject.SetActive(isExpanded);
        }
    }

    /// <summary>
    /// 手动刷新显示（供外部调用）
    /// </summary>
    public void ManualRefresh()
    {
        if (NewsHistorySystem.Instance != null)
        {
            currentHistory = NewsHistorySystem.Instance.GetVisibleHistory();
            RefreshDisplay();
        }
    }

    /// <summary>
    /// 设置展开状态
    /// </summary>
    public void SetExpanded(bool expanded)
    {
        isExpanded = expanded;
        UpdateUIState();
    }

    /// <summary>
    /// 获取当前展开状态
    /// </summary>
    public bool IsExpanded()
    {
        return isExpanded;
    }

    /// <summary>
    /// 检查按钮状态（调试用）
    /// </summary>
    public void CheckButtonStatus()
    {
        Debug.Log("=== 按钮状态检查 ===");
        Debug.Log($"isExpanded: {isExpanded}");

        if (expandButton != null)
        {
            Debug.Log($"展开按钮: 存在, 启用={expandButton.gameObject.activeInHierarchy}, 交互={expandButton.interactable}");
        }
        else
        {
            Debug.LogWarning("展开按钮: 引用为空!");
        }

        if (clearButton != null)
        {
            Debug.Log($"清除按钮: 存在, 启用={clearButton.gameObject.activeInHierarchy}, 交互={clearButton.interactable}");
        }
        else
        {
            Debug.LogWarning("清除按钮: 引用为空!");
        }

        Debug.Log("=== 按钮状态检查完成 ===");
    }

    /// <summary>
    /// 调整内容区域位置
    /// 可选择是否让内容区域移动
    /// </summary>
    private void AdjustContentAreaPosition()
    {
        if (contentParent == null) return;

        RectTransform contentRect = contentParent.GetComponent<RectTransform>();
        if (contentRect == null) return;

        // 根据配置决定是否移动content位置
        float targetY;
        // 收起状态：内容区域紧贴按钮
        targetY = 0;


        // 动画调整位置
        Vector2 currentPos = contentRect.anchoredPosition;
        Vector2 targetPos = new Vector2(currentPos.x, targetY);

        contentRect.DOAnchorPos(targetPos, expandDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Debug.Log($"内容区域位置调整完成: {contentRect.anchoredPosition}");
            });

        Debug.Log($"调整内容区域位置: 展开={isExpanded}, 从{currentPos}到{targetPos}");
    }

    /// <summary>
    /// 调整ScrollRect尺寸
    /// 确保ScrollRect的高度与展开状态匹配
    /// </summary>
    private void AdjustScrollRectSize()
    {
        if (scrollRect == null) return;

        RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        if (scrollRectTransform == null) return;

        // 计算ScrollRect的目标尺寸
        Vector2 targetSize;
        if (isExpanded)
        {
            // 展开状态：高度为总高度减去按钮栏高度
            targetSize = new Vector2(200f, expandedSize.y - headerHeight);
        }
        else
        {
            // 收起状态：高度为0（不显示内容）
            targetSize = new Vector2(200f, 0);
        }

        // 动画调整尺寸
        Vector2 currentSize = scrollRectTransform.sizeDelta;

        scrollRectTransform.DOSizeDelta(targetSize, expandDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Debug.Log($"ScrollRect尺寸调整完成: {scrollRectTransform.sizeDelta}");
            });

        Debug.Log($"调整ScrollRect尺寸: 展开={isExpanded}, 从{currentSize}到{targetSize}");
    }
}
