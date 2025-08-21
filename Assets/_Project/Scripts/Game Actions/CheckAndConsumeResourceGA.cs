using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 检查并消耗资源的GameAction
/// </summary>
public class CheckAndConsumeResourceGA : GameAction
{
    public List<ResourceCost> ResourceCosts { get; private set; }
    public List<Effect> SuccessEffects { get; private set; }
    public CharacterView CharacterView { get; private set; }
    public LineView TargetLineView { get; private set; }

    public CheckAndConsumeResourceGA(
        List<ResourceCost> resourceCosts,
        List<Effect> successEffects,
        CharacterView characterView,
        LineView targetLineView)
    {
        ResourceCosts = resourceCosts;
        SuccessEffects = successEffects;
        CharacterView = characterView;
        TargetLineView = targetLineView;
    }
}
