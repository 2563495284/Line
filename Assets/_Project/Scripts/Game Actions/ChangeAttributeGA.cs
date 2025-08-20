using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAttributeGA : GameAction
{
    public EPlayerAttributeType attributeType;
    public float attributeValue;
    public ChangeAttributeGA(EPlayerAttributeType attributeType, float attributeValue)
    {
        this.attributeType = attributeType;
        this.attributeValue = attributeValue;
    }
}