using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 天堂制造执行动作
/// 在天堂制造模式下替代NextRoundTurnGA，用于触发NPC和市场事件
/// 其他系统可以通过监听此GA来实现额外逻辑
/// </summary>
public class MadeInHeavenExecuteGA : GameAction
{
    public MadeInHeavenExecuteGA()
    {
    }
}
