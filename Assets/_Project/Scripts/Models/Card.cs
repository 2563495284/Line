using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Card
{
    private readonly CardData data;

    public string Title { get; private set; }
    public string Description { get; private set; }
    public Effect ManualTargetEffect { get; private set; }
    public List<AutoTargetEffect> OtherEffects { get; private set; }
    public Sprite Image { get; private set; }

    public int BuyStockAmount { get; private set; }

    /// <summary>
    /// Initialization of a new generic Card based on its ScriptableObject
    /// </summary>
    /// <param name="cardData"></param>
    public Card(CardData cardData)
    {
        data = cardData;
        Image = data.Image;
        Title = data.Title;
        Description = data.Description;
        ManualTargetEffect = data.ManualTargetEffect;
        OtherEffects = data.OtherEffects;
        BuyStockAmount = data.buyStockAmount;
    }
}
