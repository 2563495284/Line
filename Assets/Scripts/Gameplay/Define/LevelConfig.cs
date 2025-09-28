using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "UniverseConfig", menuName = "ConfigData/LevelConfig", order = 0)]
public class LevelConfig : ScriptableObject
{
    [Header("Player")]
    public int initialDrawCount = 5;
    public List<int> cardCfgIds = new();
    public int maxHandNum = 10;

    [Header("Level")]
    public int targetRounds = 100;
    public float targetTotalAsset = 3_000_000f;
    [Header("Debug")]
    public bool showDebugInfo = false;
    [Header("PlayerSystem")]
    public int baseManaPerTurn = 3;
    public int maxMana = 999;
    public int baseCardsPerTurn = 5;
    [Header("StockSystem")]
    public Vector2 bullImpactPercentRange = new Vector2(1.0f, 4.0f);
    public Vector2 neutralImpactPercentRange = new Vector2(-0.1f, 0.1f);
    public Vector2 bearImpactPercentRange = new Vector2(-4.0f, -1.0f);

    [Header("MarketEventSystem")]
    public int roundInterval = 2; // 每隔 N 回合尝试触发


}