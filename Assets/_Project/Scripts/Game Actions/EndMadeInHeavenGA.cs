using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 结束天堂制造模式的游戏动作
/// 执行时会让相关系统恢复对NextRoundTurnGA的订阅，取消对MadeInHeavenExecuteGA的订阅
/// </summary>
public class EndMadeInHeavenGA : GameAction
{
    public EndMadeInHeavenGA()
    {
    }
}
