using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Financial", fileName = "Financial Data")]
[Serializable]
public class FinancialData : ScriptableObject
{
    [Header("初始资金配置")]
    [SerializeField] private float initialMoney = 200000f;

    [Header("资金限制")]
    [SerializeField] private float minMoney = 0f;
    [SerializeField] private float maxMoney = float.MaxValue;

    [Header("游戏目标")]
    [SerializeField] private float targetAssets = 3000000f;
    [SerializeField] private int maxRounds = 100;

    [Header("显示格式")]
    [SerializeField] private string moneyFormat = "N0";
    [SerializeField] private string assetFormat = "N0";

    // 公共访问器
    public float InitialMoney => initialMoney;
    public float MinMoney => minMoney;
    public float MaxMoney => maxMoney;
    public float TargetAssets => targetAssets;
    public int MaxRounds => maxRounds;
    public string MoneyFormat => moneyFormat;
    public string AssetFormat => assetFormat;

    /// <summary>
    /// 验证资金是否在有效范围内
    /// </summary>
    public float ClampMoney(float amount)
    {
        return Mathf.Clamp(amount, minMoney, maxMoney);
    }

    /// <summary>
    /// 格式化金钱显示
    /// </summary>
    public string FormatMoney(float amount)
    {
        return amount.ToString(moneyFormat);
    }

    /// <summary>
    /// 格式化资产显示
    /// </summary>
    public string FormatAssets(float amount)
    {
        return amount.ToString(assetFormat);
    }

    /// <summary>
    /// 检查是否达到目标资产
    /// </summary>
    public bool HasReachedTarget(float currentAssets)
    {
        return currentAssets >= targetAssets;
    }

    /// <summary>
    /// 计算资产达成进度 (0-1)
    /// </summary>
    public float GetAssetProgress(float currentAssets)
    {
        return Mathf.Clamp01(currentAssets / targetAssets);
    }
}
