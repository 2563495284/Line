using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NextRoundEffect : Effect2
{
    public override GameAction GetGameAction()
    {
        NextRoundTurnGA nextRoundTurnGA = new();
        return nextRoundTurnGA;
    }
}