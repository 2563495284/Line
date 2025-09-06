using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 新闻历史记录项UI组件
/// 显示单条新闻的标题、内容和时间信息
/// </summary>
public class NewsHistoryItemUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image typeIcon;

    [Header("颜色设置")]
    [SerializeField] private Color infoColor = new Color(0.2f, 0.6f, 1f, 0.9f);
    [SerializeField] private Color cardPlayColor = new Color(0.8f, 0.4f, 1f, 0.9f);
    [SerializeField] private Color tradeColor = new Color(1f, 0.8f, 0.2f, 0.9f);
    [SerializeField] private Color positiveColor = new Color(0.2f, 0.8f, 0.2f, 0.9f);
    [SerializeField] private Color negativeColor = new Color(1f, 0.2f, 0.2f, 0.9f);
    [SerializeField] private Color predictionColor = new Color(0.2f, 0.8f, 0.2f, 0.9f);
    [SerializeField] private Color marketEventColor = new Color(0.6f, 0.3f, 0.8f, 0.9f);

    [Header("显示设置")]
    [SerializeField] private int maxTitleLength = 20; // 标题最大长度
    [SerializeField] private int maxContentLength = 50; // 内容最大长度
    [SerializeField] private float itemHeight = 200f; // 项目高度

    private NewsHistoryItem currentItem;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // 设置初始尺寸 - 使用配置的高度值
        rectTransform.sizeDelta = new Vector2(0, itemHeight);

        // 设置锚点
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 1);
    }

    /// <summary>
    /// 初始化新闻项
    /// </summary>
    public void Initialize(NewsHistoryItem item)
    {
        currentItem = item;
        UpdateDisplay();
    }

    /// <summary>
    /// 更新显示内容
    /// </summary>
    private void UpdateDisplay()
    {
        if (currentItem == null) return;

        // 设置标题
        if (titleText != null)
        {
            string title = currentItem.title;
            if (title.Length > maxTitleLength)
            {
                title = title.Substring(0, maxTitleLength) + "...";
            }
            titleText.text = title;
        }

        // 设置内容
        if (contentText != null)
        {
            string content = currentItem.content;
            if (content.Length > maxContentLength)
            {
                content = content.Substring(0, maxContentLength) + "...";
            }
            contentText.text = content;
        }

        // 设置时间
        if (timeText != null)
        {
            timeText.text = currentItem.GetTimeString();
        }

        // 设置背景颜色
        if (backgroundImage != null)
        {
            backgroundImage.color = GetColorByType(currentItem.newsType);
        }

        // 设置类型图标（可选）
        if (typeIcon != null)
        {
            SetTypeIcon(currentItem.newsType);
        }
    }

    /// <summary>
    /// 根据新闻类型获取颜色
    /// </summary>
    private Color GetColorByType(NewsType newsType)
    {
        switch (newsType)
        {
            case NewsType.CardPlay:
                return cardPlayColor;
            case NewsType.Trade:
                return tradeColor;
            case NewsType.Positive:
                return positiveColor;
            case NewsType.Negative:
                return negativeColor;
            case NewsType.Info:
                return infoColor;
            case NewsType.Prediction:
                return predictionColor;
            case NewsType.MarketEvent:
                return marketEventColor;
            default:
                return infoColor;
        }
    }

    /// <summary>
    /// 设置类型图标
    /// </summary>
    private void SetTypeIcon(NewsType newsType)
    {
        // 这里可以根据新闻类型设置不同的图标
        // 目前使用颜色来区分，可以后续扩展
        if (typeIcon != null)
        {
            typeIcon.color = GetColorByType(newsType);
        }
    }

    /// <summary>
    /// 获取当前新闻项
    /// </summary>
    public NewsHistoryItem GetCurrentItem()
    {
        return currentItem;
    }

    /// <summary>
    /// 设置高亮状态
    /// </summary>
    public void SetHighlighted(bool highlighted)
    {
        if (backgroundImage != null)
        {
            Color color = backgroundImage.color;
            color.a = highlighted ? 1f : 0.9f;
            backgroundImage.color = color;
        }
    }

    /// <summary>
    /// 设置选中状态
    /// </summary>
    public void SetSelected(bool selected)
    {
        if (backgroundImage != null)
        {
            Color color = backgroundImage.color;
            if (selected)
            {
                color = Color.Lerp(color, Color.white, 0.3f);
            }
            backgroundImage.color = color;
        }
    }

    /// <summary>
    /// 设置项目高度
    /// </summary>
    public void SetItemHeight(float height)
    {
        itemHeight = height;
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }
    }

    /// <summary>
    /// 获取当前项目高度
    /// </summary>
    public float GetItemHeight()
    {
        return itemHeight;
    }

    /// <summary>
    /// 获取新闻项的完整信息（用于调试）
    /// </summary>
    public string GetDebugInfo()
    {
        if (currentItem == null) return "No item";

        return $"ID: {currentItem.id}\n" +
               $"Title: {currentItem.title}\n" +
               $"Content: {currentItem.content}\n" +
               $"Type: {currentItem.newsType}\n" +
               $"Time: {currentItem.timestamp}\n" +
               $"Merged: {currentItem.isMerged}";
    }
}
