using System.Collections.Generic;
using UnityEngine;

public enum EStrategyType
{
    medium,
    aggressive,
    conservative,
}
[CreateAssetMenu(fileName = "NPCConfigItem", menuName = "ConfigData/NPCConfig", order = 0)]
public class EnemyConfigItem : ScriptableObject
{
    public int id;
    public List<int> cardIdDeck = new();
    public readonly int maxHandSize = 3;
    public readonly int initialDrawCount = 3;
    public EStrategyType strategyType;

}