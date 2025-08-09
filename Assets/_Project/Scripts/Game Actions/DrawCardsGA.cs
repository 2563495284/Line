using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardsGA : GameAction
{
    public CharacterView CharacterView { get; set; }
    public int Amount { get; set; }

    public DrawCardsGA(int amount, CharacterView characterView)
    {
        Amount = amount;
        CharacterView = characterView;
    }
}