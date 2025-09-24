using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAttributeGA : GameAction
{
    public EAttrType attributeType;
    public float attributeValue;
    public ChangeAttributeGA(EAttrType attributeType, float attributeValue)
    {
        this.attributeType = attributeType;
        this.attributeValue = attributeValue;
    }
}