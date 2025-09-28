using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameConfig;

/// <summary>
/// 单个属性显示项
/// </summary>
public class PlayerAttrItem : MonoBehaviour
{
    private AttrType type;
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI attributeNameText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image iconImage;




    /// <summary>
    /// 初始化显示项
    /// </summary>
    public void Init(AttrType type)
    {
        this.type = type;
        var attrCfg = Config.AttrConfig.Get(type);
        // 设置基础信息
        if (attributeNameText != null)
        {
            attributeNameText.text = attrCfg.Name + "：";
        }

        // 设置图标

        // 设置按钮事件
        // SetupUpgradeButton();

        UpdateValue();
    }
    public void UpdateValue()
    {
        valueText.text = GM.LevelData.GetAttrValue(type).ToString();

    }
}
