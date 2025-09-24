using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 天堂制造效果 - 让NPC和市场新闻每3秒自动执行，而不依赖手动点击下一回合
/// </summary>
public class MadeInHeavenEffect : Effect2
{
    [SerializeField] private float autoInterval = 3.0f; // 自动执行间隔时间

    public override GameAction GetGameAction()
    {
        // 直接返回启动天堂制造GA
        // 这个GA会被ActionSystem执行，然后AutoSystemManager会响应
        return new StartMadeInHeavenGA(autoInterval);
    }
}
