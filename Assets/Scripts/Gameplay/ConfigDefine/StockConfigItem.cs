using UnityEngine;

public enum EStockType
{
    Oil,      // 石油
    Steel,    // 钢铁
    Cotton    // 棉花
}
[CreateAssetMenu(fileName = "StockConfigItem", menuName = "ConfigData/StockConfigItem", order = 0)]
public class StockConfigItem : ScriptableObject
{
    public EStockType stockType;
    public string stockName = "";
    public string stockSymbol = "";
    public float initialPrice = 100f;
    public float baseVolatility = 1f; // 基础波动性
}