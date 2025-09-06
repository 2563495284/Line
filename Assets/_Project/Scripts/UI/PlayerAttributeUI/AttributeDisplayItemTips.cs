using TMPro;
using UnityEngine;

/// <summary>
/// 属性提示框单项显示（世界空间）
/// </summary>
public class AttributeDisplayItemTips : MonoBehaviour
{
    [Header("世界空间组件")]
    [SerializeField] private TextMeshProUGUI attributeNameText;
    [SerializeField] private TextMeshProUGUI attributeDescriptionText;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private SpriteRenderer iconSpriteRenderer;

    private void Awake()
    {
        // 验证世界空间组件配置
        ValidateComponents();
    }

    /// <summary>
    /// 验证组件配置
    /// </summary>
    private void ValidateComponents()
    {
        if (attributeNameText == null || attributeDescriptionText == null)
        {
            Debug.LogError($"[AttributeTooltipItem] {gameObject.name}: 缺少必要的TextMeshPro组件！请在Inspector中配置attributeNameText和attributeDescriptionText。");
        }
    }

    /// <summary>
    /// 设置属性信息
    /// </summary>
    /// <param name="attributeData">属性数据</param>
    /// <param name="isHighlighted">是否高亮显示</param>
    public void Setup(PlayerAttributeData attributeData, bool isHighlighted = false)
    {
        if (attributeData == null) return;

        // 验证组件是否存在
        if (attributeNameText == null || attributeDescriptionText == null)
        {
            Debug.LogWarning($"[AttributeTooltipItem] 组件缺失！请在Inspector中配置TextMeshPro组件。");
            return;
        }

        // 设置文本
        attributeNameText.text = attributeData.attributeName;
        attributeDescriptionText.text = attributeData.GetFormattedDescription();

        // 设置图标
        if (iconSpriteRenderer != null)
        {
            if (attributeData.icon != null)
            {
                iconSpriteRenderer.sprite = attributeData.icon;
                iconSpriteRenderer.gameObject.SetActive(true);
            }
            else
            {
                iconSpriteRenderer.gameObject.SetActive(false);
            }
        }
    }
}
