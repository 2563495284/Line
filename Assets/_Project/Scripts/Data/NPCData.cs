using System.Collections.Generic;
using UnityEngine;
public enum ECharacterStrategyType
{
    medium,
    aggressive,
    conservative,
}
[CreateAssetMenu(menuName = "Data/NPC")]
public class NPCData : CharacterData
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
}