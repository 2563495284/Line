using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源消耗系统 - 处理条件消耗效果
/// </summary>
public class ResourceConsumptionSystem : Singleton<ResourceConsumptionSystem>
{
    private void OnEnable()
    {
        // 注册条件消耗资源的处理器
        ActionSystem.AttachPerformer<CheckAndConsumeResourceGA>(CheckAndConsumeResourcePerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<CheckAndConsumeResourceGA>();
    }

    /// <summary>
    /// 处理条件消耗资源的逻辑
    /// </summary>
    private IEnumerator CheckAndConsumeResourcePerformer(CheckAndConsumeResourceGA action)
    {
        // 检查所有资源是否足够
        foreach (var cost in action.ResourceCosts)
        {
            if (!cost.CanAfford())
            {
                // 资源不足，震动相机提示
                Utils.ShakeCamera();
                TipsSystem.Instance.ShowTip("资源不足");
                Debug.Log($"资源不足: {cost.GetDescription()}");
                yield break;
            }
        }

        // 消耗所有资源
        foreach (var cost in action.ResourceCosts)
        {
            var consumeAction = cost.GetConsumeAction();
            if (consumeAction != null)
            {
                ActionSystem.Instance.Perform(consumeAction);
            }
        }

        // 执行成功后的效果
        foreach (var effect in action.SuccessEffects)
        {
            effect.SetCharacterView(action.CharacterView);
            effect.SetTargetLineView(action.TargetLineView);
            var successAction = effect.GetGameAction();
            ActionSystem.Instance.Perform(successAction);
        }

        yield return null;
    }
}
