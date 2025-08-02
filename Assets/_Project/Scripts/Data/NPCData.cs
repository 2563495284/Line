using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/NPC")]
public class NPCData : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public int maxHandSize = 3;
    [field: SerializeField] public int initialDrawCount = 3;

    [field: SerializeField] public List<CardData> CardDataList { get; private set; }
}