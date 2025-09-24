using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NewsHistoryListCom : MonoBehaviour
{
    [Header("UI组件")]

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform contentParent;
    [SerializeField] private NewsHistoryItemUI itemPrefab;
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
    private void OnExpandButtonClicked()
    {
        isExpanded = !isExpanded;
        UpdateUIState();
    }
    void Awake()
    {
        expandButton.onClick.AddListener(OnExpandButtonClicked);
    }
    public void Init()
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
                // Debug.Log($"设置内容区域初始位置: {contentRect.anchoredPosition}");
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
                // Debug.Log($"设置ScrollRect初始尺寸: {scrollRectTransform.sizeDelta}");
            }
        }

    }
    private void UpdateUIState()
    {
        Vector2 targetSize = isExpanded ? expandedSize : collapsedSize;

        // 只改变高度，保持宽度不变
        RectTransform rectTransform = GetComponent<RectTransform>();
        float currentWidth = rectTransform.sizeDelta.x;
        Vector2 newSize = new Vector2(currentWidth, targetSize.y);

        // Debug.Log($"更新UI状态: 展开={isExpanded}, 当前尺寸={rectTransform.sizeDelta}, 目标尺寸={newSize}");

        // 动画改变高度
        rectTransform.DOSizeDelta(newSize, expandDuration)
            .SetEase(Ease.OutQuad);

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

            // Debug.Log($"ScrollRect状态: 启用={scrollRect.enabled}, 垂直位置={scrollRect.verticalNormalizedPosition}");
        }

        // 调整内容区域位置
        AdjustContentAreaPosition();

    }
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
                // Debug.Log($"内容区域位置调整完成: {contentRect.anchoredPosition}");
            });

        // Debug.Log($"调整内容区域位置: 展开={isExpanded}, 从{currentPos}到{targetPos}");
    }

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
            .SetEase(Ease.OutQuad);

        // Debug.Log($"调整ScrollRect尺寸: 展开={isExpanded}, 从{currentSize}到{targetSize}");
    }
}