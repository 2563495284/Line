using UnityEngine;
using TMPro;

public class Point : MonoBehaviour
{
    [Header("点设置")]
    public SpriteRenderer spriteRenderer;
    public TextMeshPro priceLabel;

    [Header("显示设置")]
    public bool showOnHover = true; // 是否启用悬停显示
    public bool isDefaultVisible = false; // 是否默认显示

    private bool isHovered = false;
    private float originalPrice;
    private int pointIndex;

    void Start()
    {
        // 确保有SpriteRenderer
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // 确保有Collider用于鼠标检测
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one * 0.5f; // 设置合适的碰撞器大小
        }

        // 初始化价格标签显示状态
        UpdatePriceLabelVisibility();
    }

    public void RefreshPriceLabel(float price, int index, bool defaultVisible)
    {
        originalPrice = price;
        pointIndex = index;
        isDefaultVisible = defaultVisible;

        // 更新价格标签文本
        if (priceLabel != null)
        {
            priceLabel.text = price.ToString("F2");
        }

        // 更新显示状态
        UpdatePriceLabelVisibility();
    }

    void UpdatePriceLabelVisibility()
    {
        if (priceLabel != null)
        {
            // 如果默认显示，或者鼠标悬停且启用了悬停显示
            bool shouldShow = isDefaultVisible || (isHovered && showOnHover);
            priceLabel.gameObject.SetActive(shouldShow);
        }
    }

    // 鼠标进入事件
    void OnMouseEnter()
    {
        if (showOnHover)
        {
            isHovered = true;
            UpdatePriceLabelVisibility();
        }
    }

    // 鼠标离开事件
    void OnMouseExit()
    {
        if (showOnHover)
        {
            isHovered = false;
            UpdatePriceLabelVisibility();
        }
    }

    // 公共方法：设置颜色
    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    // 公共方法：设置大小
    public void SetSize(float size)
    {
        transform.localScale = Vector3.one * size;
    }

    // 公共方法：设置价格标签颜色
    public void SetLabelColor(Color color)
    {
        if (priceLabel != null)
        {
            priceLabel.color = color;
        }
    }

    // 公共方法：强制显示/隐藏价格标签
    public void SetLabelVisible(bool visible)
    {
        if (priceLabel != null)
        {
            priceLabel.gameObject.SetActive(visible);
        }
    }

    // 公共方法：获取点信息
    public float GetPrice() => originalPrice;
    public int GetIndex() => pointIndex;
    public bool IsHovered() => isHovered;
}
