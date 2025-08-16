using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 升级玩家属性的GameAction
/// </summary>
public class UpgradeAttributeGA : GameAction
{
    public EPlayerAttributeType AttributeType { get; set; }

    public UpgradeAttributeGA(EPlayerAttributeType attributeType)
    {
        AttributeType = attributeType;
    }
}
