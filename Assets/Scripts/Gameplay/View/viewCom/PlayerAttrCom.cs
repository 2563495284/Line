using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 玩家属性显示UI
/// </summary>
public class PlayerAttrCom : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private GameObject attributeItemPrefab;

    [Header("状态显示")]
    [SerializeField] private TextMeshProUGUI cardsPerTurnText;

    private List<PlayerAttrItem> attributeDisplayItems = new List<PlayerAttrItem>();

    public void Init()
    {
        foreach (EAttrType t in GM.Ins.Level.model.attrVals.Keys)
        {
            GameObject itemObj = Instantiate(attributeItemPrefab, transform);
            PlayerAttrItem displayItem = itemObj.GetComponent<PlayerAttrItem>();

            if (displayItem != null)
            {
                displayItem.Init(t);
                attributeDisplayItems.Add(displayItem);
            }
        }
    }

    /// <summary>
    /// 更新所有显示
    /// </summary>
    public void UpdateAllDisplays()
    {

        // 更新各个属性显示
        foreach (var displayItem in attributeDisplayItems)
        {
            displayItem.UpdateValue();
        }
        cardsPerTurnText.text = $"每回合摸牌: {GM.LevelData.CardNumPerTurn}";

    }

}
