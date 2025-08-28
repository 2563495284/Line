using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 启动天堂制造模式的游戏动作
/// 执行时会让相关系统取消对NextRoundTurnGA的订阅，改为订阅MadeInHeavenExecuteGA
/// </summary>
public class StartMadeInHeavenGA : GameAction
{
    public float AutoInterval { get; private set; }

    public StartMadeInHeavenGA(float autoInterval = 3.0f)
    {
        AutoInterval = autoInterval;
    }
}
