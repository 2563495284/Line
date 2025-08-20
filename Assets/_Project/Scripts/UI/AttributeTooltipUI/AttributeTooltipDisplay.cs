using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 属性提示框显示系统
/// </summary>
public class AttributeTooltipDisplay : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Transform tooltipContainer;
    [SerializeField] private GameObject tooltipItemPrefab;

    [Header("动画设置")]
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float fadeOutDuration = 0.15f;
    [SerializeField] private Ease fadeEase = Ease.OutQuad;

    [Header("布局设置")]
    [SerializeField] private float itemSpacing = 0.5f; // 世界空间单位
    [SerializeField] private Vector3 tooltipOffset = new Vector3(3f, 0f, 0f); // 世界坐标偏移（卡牌右侧）

    private List<AttributeTooltipItem> tooltipItems = new List<AttributeTooltipItem>();
    private List<EPlayerAttributeType> currentHighlightedAttributes = new List<EPlayerAttributeType>();
    private Tween currentTween;

    // 位置跟踪
    private bool isVisible = false;
    private Vector3 lastCardPosition;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示属性提示框
    /// </summary>
    /// <param name="attributeTypes">要显示的属性类型列表</param>
    /// <param name="highlightedAttributes">需要高亮的属性类型列表</param>
    /// <param name="worldPosition">世界坐标位置</param>
    public void Show(List<EPlayerAttributeType> attributeTypes, List<EPlayerAttributeType> highlightedAttributes, Vector3 worldPosition)
    {
        if (attributeTypes == null || attributeTypes.Count == 0) return;

        currentHighlightedAttributes = highlightedAttributes ?? new List<EPlayerAttributeType>();

        // 清理现有项目
        ClearTooltipItems();

        // 创建新的提示项目
        CreateTooltipItems(attributeTypes);

        // 设置位置
        lastCardPosition = worldPosition;
        SetTooltipPosition(worldPosition);

        // 显示动画
        ShowWithAnimation();
    }

    /// <summary>
    /// 隐藏属性提示框
    /// </summary>
    public void Hide()
    {
        isVisible = false;
        HideWithAnimation();
    }

    /// <summary>
    /// 更新提示框位置
    /// </summary>
    /// <param name="newCardPosition">新的卡牌位置</param>
    public void UpdatePosition(Vector3 newCardPosition)
    {
        if (!isVisible) return;

        lastCardPosition = newCardPosition;
        SetTooltipPosition(newCardPosition);
    }

    /// <summary>
    /// 创建提示框项目
    /// </summary>
    private void CreateTooltipItems(List<EPlayerAttributeType> attributeTypes)
    {
        if (tooltipItemPrefab == null || tooltipContainer == null) return;

        var playerAttributes = PlayerAttributeSystem.Instance?.GetPlayerAttributes();
        if (playerAttributes == null) return;

        float currentYOffset = 0f;

        foreach (var attributeType in attributeTypes)
        {
            var attributeData = playerAttributes.GetAttribute(attributeType);
            if (attributeData == null) continue;

            // 创建提示项目
            GameObject itemObj = Instantiate(tooltipItemPrefab, tooltipContainer);
            AttributeTooltipItem tooltipItem = itemObj.GetComponent<AttributeTooltipItem>();

            if (tooltipItem != null)
            {
                bool isHighlighted = currentHighlightedAttributes.Contains(attributeType);
                tooltipItem.Setup(attributeData, isHighlighted);

                // 设置垂直布局位置（从上往下）
                Vector3 localPos = itemObj.transform.localPosition;
                localPos.y = -currentYOffset;
                itemObj.transform.localPosition = localPos;

                tooltipItems.Add(tooltipItem);
                currentYOffset += itemSpacing;
            }
        }
    }

    /// <summary>
    /// 清理提示框项目
    /// </summary>
    private void ClearTooltipItems()
    {
        foreach (var item in tooltipItems)
        {
            if (item != null && item.gameObject != null)
            {
                DestroyImmediate(item.gameObject);
            }
        }
        tooltipItems.Clear();
    }

    /// <summary>
    /// 设置提示框位置（世界坐标系）
    /// </summary>
    private void SetTooltipPosition(Vector3 worldPosition)
    {
        // 直接使用世界坐标，添加偏移量
        Vector3 targetPosition = worldPosition + tooltipOffset;

        // 调试信息
        Debug.Log($"[AttributeTooltipDisplay] 卡牌位置: {worldPosition}, 偏移量: {tooltipOffset}, 目标位置: {targetPosition}");

        // 设置位置
        transform.position = targetPosition;

        // 确保提示框朝向摄像机（如果需要）
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0); // 翻转以正确显示
        }
    }

    /// <summary>
    /// 显示动画
    /// </summary>
    private void ShowWithAnimation()
    {
        isVisible = true;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏动画
    /// </summary>
    private void HideWithAnimation()
    {
        gameObject.SetActive(false);
        ClearTooltipItems();
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
    }
}
