using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// 单个属性显示项
/// </summary>
public class AttributeDisplayItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI attributeNameText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image iconImage;
    [SerializeField] public AttributeDisplayItemTips attributeDisplayItemTips;


    private PlayerAttributeData attributeData;
    private bool isMouseOver = false;

    #region Initialization

    /// <summary>
    /// 初始化显示项
    /// </summary>
    public void Initialize(PlayerAttributeData data)
    {
        attributeData = data;

        // 设置基础信息
        if (attributeNameText != null)
        {
            attributeNameText.text = attributeData.attributeName + "：";
        }

        // 设置图标
        if (iconImage != null && attributeData.icon != null)
        {
            iconImage.sprite = attributeData.icon;
        }

        // 设置按钮事件
        // SetupUpgradeButton();

        // 初始更新
        UpdateDisplay();
    }

    #endregion

    #region Mouse Events

    /// <summary>
    /// 鼠标进入事件
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;

        // 立即显示tooltip
        ShowTooltip();
    }

    /// <summary>
    /// 鼠标离开事件
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        HideTooltip();

    }

    #endregion

    #region Tooltip Management

    /// <summary>
    /// 延迟隐藏tooltip的协程
    /// </summary>
    private System.Collections.IEnumerator HideTooltipAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        // 如果鼠标仍然不在上面，则隐藏tooltip
        if (!isMouseOver)
        {
            ShowTooltip();
        }
    }

    /// <summary>
    /// 显示tooltip
    /// </summary>
    private void ShowTooltip()
    {
        if (attributeData == null || attributeDisplayItemTips == null) return;

        // 设置tooltip内容
        attributeDisplayItemTips.Setup(attributeData);

        // 显示tooltip
        attributeDisplayItemTips.gameObject.SetActive(true);

        // 修正位置，确保不超出屏幕边界
        CorrectTooltipPosition();
    }

    /// <summary>
    /// 修正tooltip位置，确保在屏幕边界内
    /// </summary>
    private void CorrectTooltipPosition()
    {
        if (attributeDisplayItemTips == null) return;

        // 获取Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        // 获取屏幕尺寸
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // 获取attributeDisplayItemTips的RectTransform
        RectTransform tooltipRect = attributeDisplayItemTips.GetComponent<RectTransform>();
        if (tooltipRect == null) return;

        // 计算tooltip在屏幕上的位置
        Vector3[] corners = new Vector3[4];
        tooltipRect.GetWorldCorners(corners);

        // 将世界坐标转换为屏幕坐标
        Vector2 minScreenPos = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 maxScreenPos = new Vector2(float.MinValue, float.MinValue);

        for (int i = 0; i < 4; i++)
        {
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[i]);
            minScreenPos.x = Mathf.Min(minScreenPos.x, screenPos.x);
            minScreenPos.y = Mathf.Min(minScreenPos.y, screenPos.y);
            maxScreenPos.x = Mathf.Max(maxScreenPos.x, screenPos.x);
            maxScreenPos.y = Mathf.Max(maxScreenPos.y, screenPos.y);
        }

        // 只计算x方向的位置偏移，y方向保持不变
        float offsetX = 0f;

        // 检查右边界 - 如果超出右边界，只向左偏移刚好让tooltip完全显示的最小距离
        if (maxScreenPos.x > screenSize.x)
        {
            // 计算超出右边界的距离，只偏移这个距离加上一点边距
            float overflowRight = maxScreenPos.x - screenSize.x;

            // 向左偏移，但限制最大偏移量，避免跑到左边
            float calculatedOffset = overflowRight + 10f;
            float maxAllowedOffset = screenSize.x * 0.2f;
            offsetX = -Mathf.Min(calculatedOffset, maxAllowedOffset);
        }

        // 检查左边界 - 如果超出左边界，向右偏移但不要过度偏移
        if (minScreenPos.x < 0)
        {
            // 计算超出左边界的距离
            float overflowLeft = -minScreenPos.x;
            // 向右偏移，但保留一些可见性，不要完全移到右边
            offsetX = overflowLeft + 10f;
        }

        // 只应用x方向的偏移
        if (offsetX != 0f)
        {
            // 简化坐标转换，直接使用屏幕偏移量
            // 对于UI Canvas，通常屏幕偏移量就等于本地偏移量
            Vector2 currentPos = tooltipRect.anchoredPosition;
            Vector2 newPos = new Vector2(currentPos.x + offsetX, currentPos.y);

            tooltipRect.anchoredPosition = newPos;
        }
    }

    /// <summary>
    /// 重置tooltip位置到原始位置
    /// </summary>
    private void ResetTooltipPosition()
    {
        if (attributeDisplayItemTips == null) return;

        RectTransform tooltipRect = attributeDisplayItemTips.GetComponent<RectTransform>();
        if (tooltipRect == null) return;

        // 只重置x位置，保持y位置不变
        Vector2 currentPos = tooltipRect.anchoredPosition;
        tooltipRect.anchoredPosition = new Vector2(0f, currentPos.y);
    }

    /// <summary>
    /// 隐藏tooltip
    /// </summary>
    private void HideTooltip()
    {
        if (attributeDisplayItemTips == null) return;

        // 隐藏tooltip
        attributeDisplayItemTips.gameObject.SetActive(false);

        // 重置tooltip位置，避免位置累积偏移
        ResetTooltipPosition();
    }

    #endregion

    #region Display Update

    /// <summary>
    /// 更新显示
    /// </summary>
    public void UpdateDisplay()
    {
        if (attributeData == null) return;


        // 更新数值
        UpdateValue();
    }


    private void UpdateValue()
    {
        if (valueText != null)
        {
            valueText.text = attributeData.currentValue.ToString();
        }
    }

    #endregion

    #region Cleanup

    private void OnDestroy()
    {
        // 确保在销毁时隐藏tooltip
        if (isMouseOver)
        {
            HideTooltip();
        }
    }

    #endregion
}
