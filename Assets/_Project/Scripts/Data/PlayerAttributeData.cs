using System;
using System.Collections.Generic;
using UnityEngine;
using GameConfig;

/// <summary>
/// 玩家属性类型
/// </summary>

/// <summary>
/// 单个属性数据
/// </summary>
[System.Serializable]
public class PlayerAttributeData
{
    [Header("基础信息")]
    public AttrType attributeType;
    public string attributeName;
    public string description;
    public Sprite icon;

    [Header("数值信息")]
    public float currentValue;
    public float originValue;
    public PlayerAttributeData(AttrType type)
    {
        attributeType = type;
        SetupAttributeInfo();
    }

    /// <summary>
    /// 设置属性基础信息
    /// </summary>
    private void SetupAttributeInfo()
    {
        switch (attributeType)
        {
            case AttrType.Social://基础的
                attributeName = "社交";
                description = "每回合摸牌数+1，每回合失去1";
                currentValue = 0f;
                break;
            case AttrType.Wisdom://中级
                attributeName = "智慧";
                description = "每回合能量恢复+1";
                currentValue = 0f;
                break;
            case AttrType.Charisma://中级
                attributeName = "魅力";
                description = "玩家影响力+10%";
                currentValue = 0f;
                break;
            case AttrType.Courage://基础
                attributeName = "勇气";
                description = "玩家交易数量+10%";
                currentValue = 0f;
                break;
            case AttrType.Calmness://中性
                attributeName = "冷静";
                description = "环境对市场价格影响-2%";
                currentValue = 0f;
                break;
            case AttrType.Fanaticism://中性
                attributeName = "狂热";
                description = "环境对市场价格影响+2%";
                currentValue = 0f;
                break;
        }
    }


    /// <summary>
    /// 获取格式化的描述
    /// </summary>
    public string GetFormattedDescription()
    {
        return description;
    }

    /// <summary>
    /// 获取属性显示字符串
    /// </summary>
    public string GetDisplayString()
    {
        return $"{attributeName} ({currentValue})";
    }
}

/// <summary>
/// 玩家属性系统数据
/// </summary>
[System.Serializable]
public class PlayerAttributesData
{
    [Header("属性列表")]
    public List<PlayerAttributeData> attributes = new List<PlayerAttributeData>();

    [Header("统计信息")]
    public int totalAttributePoints;
    public int totalMoneySpent;

    public PlayerAttributesData()
    {
        InitializeAttributes();
    }

    /// <summary>
    /// 初始化所有属性
    /// </summary>
    private void InitializeAttributes()
    {
        attributes.Clear();

        foreach (AttrType attributeType in Enum.GetValues(typeof(AttrType)))
        {
            attributes.Add(new PlayerAttributeData(attributeType));
        }

    }

    /// <summary>
    /// 获取指定属性
    /// </summary>
    public PlayerAttributeData GetAttribute(AttrType type)
    {
        return attributes.Find(attr => attr.attributeType == type);
    }

    /// <summary>
    /// 获取属性值
    /// </summary>
    public float GetAttributeValue(AttrType type)
    {
        var attribute = GetAttribute(type);
        return attribute?.currentValue ?? 0f;
    }

    /// <summary>
    /// 获取总属性加成描述
    /// </summary>
    public List<string> GetAllEffectDescriptions()
    {
        List<string> descriptions = new List<string>();

        foreach (var attribute in attributes)
        {
            if (attribute.currentValue > 0)
            {
                descriptions.Add($"{attribute.attributeName}: {attribute.GetFormattedDescription()}");
            }
        }

        return descriptions;
    }
}
