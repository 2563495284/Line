using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCardGA : GameAction
{
    public Card Card { get; set; }

    public CharacterView CharacterView { get; set; }

    public LineView LineViewTarget { get; set; }

    public PlayCardGA(Card card, CharacterView characterView)
    {
        Card = card;
        CharacterView = characterView;
    }

    public PlayCardGA(Card card, CharacterView characterView, LineView lineViewTarget)
    {
        Card = card;
        CharacterView = characterView;
        LineViewTarget = lineViewTarget;
    }
}