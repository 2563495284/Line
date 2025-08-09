using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardAllCardsGA : GameAction
{
    public CharacterView CharacterView { get; set; }

    public DiscardAllCardsGA(CharacterView characterView)
    {
        CharacterView = characterView;
    }
}