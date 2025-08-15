using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触发事件的Game Action
/// </summary>
public class TriggerEventGA : GameAction
{
    public CardData EventCard { get; set; }

    public TriggerEventGA(CardData eventCard)
    {
        EventCard = eventCard;
    }
}