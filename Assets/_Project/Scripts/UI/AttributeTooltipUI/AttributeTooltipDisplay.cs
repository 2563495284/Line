using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using GameConfig;
/// <summary>
/// 属性提示框显示系统
/// </summary>
public class AttributeTooltipDisplay : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Transform tooltipContainer;
    [SerializeField] private GameObject tooltipItemPrefab;

    private List<AttributeTooltipItem> tooltipItems = new List<AttributeTooltipItem>();
    private List<AttrType> currentHighlightedAttributes = new List<AttrType>();
    private Tween currentTween;

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
    public void Show(List<AttrType> attributeTypes, List<AttrType> highlightedAttributes, Vector3 worldPosition)
    {
        if (attributeTypes == null || attributeTypes.Count == 0) return;

        currentHighlightedAttributes = highlightedAttributes ?? new List<AttrType>();

        // 清理现有项目
        ClearTooltipItems();

        // 创建新的提示项目
        CreateTooltipItems(attributeTypes);

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏属性提示框
    /// </summary>
    public void Hide()
    {
        ClearTooltipItems();
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 创建提示框项目
    /// </summary>
    private void CreateTooltipItems(List<AttrType> attributeTypes)
    {
        if (tooltipItemPrefab == null || tooltipContainer == null) return;

        var playerAttributes = PlayerAttributeSystem.Ins?.GetPlayerAttributes();
        if (playerAttributes == null) return;

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

    private void OnDestroy()
    {
        currentTween?.Kill();
    }
}
