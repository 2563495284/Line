using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/CharacterData")]
public class CharacterData : ScriptableObject
{
    [field: SerializeField] public List<CardData> Deck { get; private set; }
    [field: SerializeField] public int maxHandSize { get; private set; }
    [field: SerializeField] public int initialDrawCount { get; private set; }
    [field: SerializeField] public ECharacterType CharacterType { get; private set; }
    [field: SerializeField] public EStrategyType StrategyType { get; private set; }

}