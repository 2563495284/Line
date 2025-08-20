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
                tooltipItems.Add(tooltipItem);
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
