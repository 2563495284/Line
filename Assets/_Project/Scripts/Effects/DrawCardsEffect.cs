using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardsEffect : Effect2
{
    [SerializeField] private int drawAmount;

    public override GameAction GetGameAction()
    {
        DrawCardsGA drawCardsGA = new(drawAmount, characterView);
        return drawCardsGA;
    }
}