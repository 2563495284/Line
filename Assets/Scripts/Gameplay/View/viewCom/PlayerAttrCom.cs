using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameConfig;

/// <summary>
/// 玩家属性显示UI
/// </summary>
public class PlayerAttrCom : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private GameObject attributeItemPrefab;


    private List<PlayerAttrItem> attributeDisplayItems = new List<PlayerAttrItem>();

    public void Init()
    {
        foreach (AttrType t in GM.Ins.Level.model.attrVals.Keys)
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

    }

}
