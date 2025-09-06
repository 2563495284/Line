using UnityEngine;
using TMPro;
using JetBrains.Annotations;

// 点位状态枚举，使用2的幂次方便于位运算
[System.Flags]
public enum PointState
{
    None = 0,           // 无状态
    Buy = 1,            // 买入 (001)
    Sell = 2,           // 卖出 (010)
    Bullish = 4,        // 看多 (100)
    Bearish = 8         // 看空 (1000)
}

public class Point : MonoBehaviour
{
    [Header("点设置")]
    public SpriteRenderer spriteRenderer;
    public TextMeshPro priceLabel;
    public TextMeshPro stateLabel; // 状态标签

    [Header("显示设置")]
    public bool showOnHover = true; // 是否启用悬停显示
    public bool isDefaultVisible = false; // 是否默认显示

    [Header("状态颜色设置")]
    public Color buyColor = Color.green;           // 买入颜色
    public Color sellColor = Color.red;            // 卖出颜色
    public Color buySellColor = Color.yellow;      // 买卖颜色
    public Color bullishColor = Color.blue;        // 看多颜色
    public Color bearishColor = new Color(0.5f, 0f, 0.5f);      // 看空颜色（紫色）
    public Color bullishBearishColor = new Color(1f, 0.5f, 0f); // 多空颜色（橙色）
    public Color defaultColor = Color.black;       // 默认颜色

    [Header("状态文字设置")]

    public string defaultText = "";
    public string buyText = "买";
    public string sellText = "卖";
    public string buySellText = "买卖";
    public string bullishText = "多";
    public string bearishText = "空";
    public string bullishBearishText = "多空";

    private bool isHovered = false;
    private float originalPrice;
    private int pointIndex;
    private PointState currentState = PointState.None;
    private float originalSize = 1f; // 保存原始大小

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

        // 初始化状态标签
        InitializeStateLabel();

        // 初始化价格标签显示状态
        UpdatePriceLabelVisibility();
    }

    public void InitializeStateLabel()
    {
        SetState(PointState.None);
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

    // 添加状态（位运算OR）
    public void AddState(PointState stateToAdd)
    {
        currentState |= stateToAdd;
        UpdateVisualState();
    }

    // 移除状态（位运算AND NOT）
    public void RemoveState(PointState stateToRemove)
    {
        currentState &= ~stateToRemove;
        UpdateVisualState();
    }

    // 设置状态（直接赋值）
    public void SetState(PointState newState)
    {
        currentState = newState;
        UpdateVisualState();
    }

    // 清除所有状态
    public void ClearState()
    {
        currentState = PointState.None;
        UpdateVisualState();
    }

    // 检查是否包含某个状态
    public bool HasState(PointState state)
    {
        return (currentState & state) == state;
    }

    // 获取当前状态
    public PointState GetCurrentState()
    {
        return currentState;
    }

    // 更新视觉状态
    void UpdateVisualState()
    {
        if (spriteRenderer == null) return;

        // 检查组合状态
        bool hasBuy = HasState(PointState.Buy);
        bool hasSell = HasState(PointState.Sell);
        bool hasBullish = HasState(PointState.Bullish);
        bool hasBearish = HasState(PointState.Bearish);

        // 优先级：买卖组合 > 多空组合 > 单个状态
        if (hasBuy && hasSell)
        {
            // 买卖组合
            spriteRenderer.color = buySellColor;
            if (stateLabel != null) stateLabel.text = buySellText;
            transform.localScale = Vector3.one * originalSize; // 使用原始大小
        }
        else if (hasBullish && hasBearish)
        {
            // 多空组合
            spriteRenderer.color = bullishBearishColor;
            if (stateLabel != null) stateLabel.text = bullishBearishText;
            transform.localScale = Vector3.one * originalSize; // 使用原始大小
        }
        else if (hasBuy)
        {
            // 只有买入
            spriteRenderer.color = buyColor;
            if (stateLabel != null) stateLabel.text = buyText;
            transform.localScale = Vector3.one * originalSize; // 使用原始大小
        }
        else if (hasSell)
        {
            // 只有卖出
            spriteRenderer.color = sellColor;
            if (stateLabel != null) stateLabel.text = sellText;
            transform.localScale = Vector3.one * originalSize; // 使用原始大小
        }
        else if (hasBullish)
        {
            // 只有看多
            spriteRenderer.color = bullishColor;
            if (stateLabel != null) stateLabel.text = bullishText;
            transform.localScale = Vector3.one * originalSize; // 使用原始大小
        }
        else if (hasBearish)
        {
            // 只有看空
            spriteRenderer.color = bearishColor;
            if (stateLabel != null) stateLabel.text = bearishText;
            transform.localScale = Vector3.one * originalSize; // 使用原始大小
        }
        else
        {
            // 无状态
            spriteRenderer.color = defaultColor;
            if (stateLabel != null) stateLabel.text = defaultText;
            transform.localScale = Vector3.one * (originalSize * 0.5f); // 空状态缩小为原始大小的0.5倍
        }
    }

    // 便捷方法：添加买入状态
    public void AddBuyState() => AddState(PointState.Buy);

    // 便捷方法：添加卖出状态
    public void AddSellState() => AddState(PointState.Sell);

    // 便捷方法：添加看多状态
    public void AddBullishState() => AddState(PointState.Bullish);

    // 便捷方法：添加看空状态
    public void AddBearishState() => AddState(PointState.Bearish);

    // 便捷方法：移除买入状态
    public void RemoveBuyState() => RemoveState(PointState.Buy);

    // 便捷方法：移除卖出状态
    public void RemoveSellState() => RemoveState(PointState.Sell);

    // 便捷方法：移除看多状态
    public void RemoveBullishState() => RemoveState(PointState.Bullish);

    // 便捷方法：移除看空状态
    public void RemoveBearishState() => RemoveState(PointState.Bearish);

    // 便捷方法：设置买入状态（清除其他状态）
    public void SetBuyState() => SetState(PointState.Buy);

    // 便捷方法：设置卖出状态（清除其他状态）
    public void SetSellState() => SetState(PointState.Sell);

    // 便捷方法：设置看多状态（清除其他状态）
    public void SetBullishState() => SetState(PointState.Bullish);

    // 便捷方法：设置看空状态（清除其他状态）
    public void SetBearishState() => SetState(PointState.Bearish);

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
        originalSize = size; // 保存原始大小
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
