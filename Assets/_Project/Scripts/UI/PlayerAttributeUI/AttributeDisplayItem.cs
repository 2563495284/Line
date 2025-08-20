using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 单个属性显示项
/// </summary>
public class AttributeDisplayItem : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI attributeNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    private PlayerAttributeData attributeData;

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
            attributeNameText.text = attributeData.attributeName;
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

    #region Display Update

    /// <summary>
    /// 更新显示
    /// </summary>
    public void UpdateDisplay()
    {
        if (attributeData == null) return;

        // 更新描述
        UpdateDescription();

        // 更新数值
        UpdateValue();
    }

    /// <summary>
    /// 更新描述显示
    /// </summary>
    private void UpdateDescription()
    {
        if (descriptionText != null)
        {
            descriptionText.text = attributeData.GetFormattedDescription();
        }
    }
    private void UpdateValue()
    {
        if (valueText != null)
        {
            valueText.text = attributeData.currentValue.ToString();
        }
    }
    #endregion
}
