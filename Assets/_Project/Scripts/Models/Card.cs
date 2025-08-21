using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    private readonly CardData data;

    public string Title { get; private set; }
    public string Description { get; private set; }
    public string RichTextDescription { get; private set; }
    public Effect ManualTargetEffect { get; private set; }
    public List<AutoTargetEffect> OtherEffects { get; private set; }
    public Sprite Image { get; private set; }

    public int Mana { get; private set; }

    // 动态描述相关属性
    public List<EPlayerAttributeType> ReferencedAttributes { get; private set; }

    /// <summary>
    /// Initialization of a new generic Card based on its ScriptableObject
    /// </summary>
    /// <param name="cardData"></param>
    public Card(CardData cardData)
    {
        data = cardData;
        Image = data.Image;
        Title = data.Title;
        ManualTargetEffect = data.ManualTargetEffect;
        OtherEffects = data.OtherEffects;
        Mana = data.Mana;

        // 初始化动态描述
        UpdateDescription();
    }

    /// <summary>
    /// 更新卡牌描述（包含动态数值）
    /// </summary>
    public void UpdateDescription()
    {
        var result = CardDescriptionSystem.ProcessCardDescription(data.Description, this);
        Description = result.processedDescription;
        RichTextDescription = result.richTextDescription;
        ReferencedAttributes = result.referencedAttributes;
    }

    /// <summary>
    /// 获取原始描述（来自ScriptableObject）
    /// </summary>
    public string GetOriginalDescription()
    {
        return data.Description;
    }
}
