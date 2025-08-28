using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NextRoundEffect : Effect
{
    public override GameAction GetGameAction()
    {
        NextRoundTurnGA nextRoundTurnGA = new();
        return nextRoundTurnGA;
    }
}