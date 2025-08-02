using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player")]
public class PlayerData : ScriptableObject
{
    [field: SerializeField] public List<CardData> Deck { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
}