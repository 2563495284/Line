using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformEffectGA : GameAction
{
    public Effect2 Effect { get; set; }

    public PerformEffectGA(Effect2 effect, CharacterView characterView, LineView targetLineView)
    {
        Effect = effect;
        Effect.SetCharacterView(characterView);
        Effect.SetTargetLineView(targetLineView);
    }
    public PerformEffectGA(Effect2 effect, CharacterView characterView)
    {
        Effect = effect;
        Effect.SetCharacterView(characterView);
    }

}