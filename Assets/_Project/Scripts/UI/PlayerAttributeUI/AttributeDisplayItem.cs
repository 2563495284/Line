using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Image iconImage;

    [Header("颜色设置")]
    [SerializeField] private Color canUpgradeColor = Color.green;
    [SerializeField] private Color cannotUpgradeColor = Color.gray;
    [SerializeField] private Color maxLevelColor = Color.yellow;

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

    /// <summary>
    /// 设置升级按钮
    /// </summary>
    private void SetupUpgradeButton()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(UpgradeAttribute);
        }
    }

    #endregion

    #region Display Update

    /// <summary>
    /// 更新显示
    /// </summary>
    public void UpdateDisplay()
    {
        if (attributeData == null) return;

        // 更新等级和数值
        UpdateLevelAndValue();

        // 更新描述
        UpdateDescription();

        // 更新升级信息
        // UpdateUpgradeInfo();
    }

    /// <summary>
    /// 更新等级和数值显示
    /// </summary>
    private void UpdateLevelAndValue()
    {
        if (levelText != null)
        {
            levelText.text = $"Lv.{attributeData.currentLevel}";

            // 设置颜色
            if (attributeData.currentLevel >= attributeData.maxLevel)
            {
                levelText.color = maxLevelColor;
            }
        }

        if (valueText != null)
        {
            valueText.text = $"{attributeData.currentValue}";
        }
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

    /// <summary>
    /// 更新升级信息
    /// </summary>
    private void UpdateUpgradeInfo()
    {
        bool canUpgrade = CanUpgradeAttribute();
        bool isMaxLevel = attributeData.currentLevel >= attributeData.maxLevel;

        // 更新升级费用显示
        if (upgradeCostText != null)
        {
            if (isMaxLevel)
            {
                upgradeCostText.text = "已满级";
                upgradeCostText.color = maxLevelColor;
            }
            else
            {
                int cost = attributeData.GetNextUpgradeCost();
                upgradeCostText.text = $"升级: {cost}金币";
                upgradeCostText.color = canUpgrade ? canUpgradeColor : cannotUpgradeColor;
            }
        }

        // 更新升级按钮
        if (upgradeButton != null)
        {
            upgradeButton.interactable = canUpgrade && !isMaxLevel;

            // 设置按钮颜色
            var buttonImage = upgradeButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (isMaxLevel)
                    buttonImage.color = maxLevelColor;
                else if (canUpgrade)
                    buttonImage.color = canUpgradeColor;
                else
                    buttonImage.color = cannotUpgradeColor;
            }
        }
    }

    #endregion

    #region Button Actions

    /// <summary>
    /// 升级属性
    /// </summary>
    private void UpgradeAttribute()
    {
        if (PlayerAttributeSystem.Instance == null) return;

        var upgradeGA = new UpgradeAttributeGA(attributeData.attributeType);
        ActionSystem.Instance.Perform(upgradeGA);
    }

    /// <summary>
    /// 检查是否可以升级
    /// </summary>
    private bool CanUpgradeAttribute()
    {
        if (PlayerAttributeSystem.Instance == null) return false;
        return PlayerAttributeSystem.Instance.CanUpgradeAttribute(attributeData.attributeType);
    }

    #endregion

    #region Context Menu Actions

    /// <summary>
    /// 强制升级（测试用）
    /// </summary>
    [ContextMenu("强制升级")]
    public void ForceUpgrade()
    {
        UpgradeAttribute();
    }

    /// <summary>
    /// 打印属性信息
    /// </summary>
    [ContextMenu("打印属性信息")]
    public void PrintAttributeInfo()
    {
        if (attributeData != null)
        {
            Debug.Log($"=== {attributeData.attributeName} ===");
            Debug.Log($"等级: {attributeData.currentLevel}/{attributeData.maxLevel}");
            Debug.Log($"数值: {attributeData.currentValue}");
            Debug.Log($"描述: {attributeData.GetFormattedDescription()}");
            Debug.Log($"升级费用: {attributeData.GetNextUpgradeCost()}");
            Debug.Log($"可升级: {CanUpgradeAttribute()}");
        }
    }

    #endregion
}
