using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameConfig;

/// <summary>
/// 单个属性显示项
/// </summary>
public class PlayerAttrItem : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI attributeNameText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image iconImage;


    public void SetData(PlayerAttrData info)
    {

    }

}
