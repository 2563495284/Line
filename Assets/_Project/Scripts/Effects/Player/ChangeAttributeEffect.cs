using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameConfig;

/// <summary>
/// 使用能量的效果
/// </summary>
public class ChangeAttributeEffect : Effect2
{
    [Header("属性消耗设置")]
    [SerializeField]
    private AttrType attributeType;

    [SerializeField]
    private float attributeValue;

    public override GameAction GetGameAction()
    {
        ChangeAttributeGA attributeGA = new ChangeAttributeGA(attributeType, attributeValue);
        return attributeGA;
    }
}
