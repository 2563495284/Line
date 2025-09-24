using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// 单个属性显示项
/// </summary>
public class PlayerAttrItem : MonoBehaviour
{
    private EAttrType type;
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI attributeNameText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image iconImage;




    /// <summary>
    /// 初始化显示项
    /// </summary>
    public void Init(EAttrType type)
    {
        this.type = type;
        var attrCfg = Global.Ins.GetAttrCfg(type);
        // 设置基础信息
        if (attributeNameText != null)
        {
            attributeNameText.text = attrCfg.attributeName + "：";
        }

        // 设置图标
        if (iconImage != null && attrCfg.icon != null)
        {
            iconImage.sprite = attrCfg.icon;
        }

        // 设置按钮事件
        // SetupUpgradeButton();

        UpdateValue();
    }
    public void UpdateValue()
    {
        valueText.text = GM.LevelData.GetAttrValue(type).ToString();

    }
}
