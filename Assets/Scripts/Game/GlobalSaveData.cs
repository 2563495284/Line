using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

/// <summary>
/// 全局存档数据 —— 持久化到磁盘，跨局携带。
/// 包含金币、材料、已建造建筑等玩家永久资产。
/// </summary>
[Serializable]
public class GlobalSaveData
{
    public string playerName = "Player";

    /// <summary>全局随机种子，新建存档时生成一次，之后固定不变</summary>
    public int randomSeed = 0;

    /// <summary>BigInteger 不支持 JsonUtility，序列化为字符串</summary>
    public string moneyStr = "0";

    /// <summary>Dictionary 不支持 JsonUtility，序列化为 List</summary>
    public List<ItemEntry> items = new List<ItemEntry>();

    public string saveTime = "";

    // ── 运行时缓存，不参与序列化 ──────────────────────────────────────────────
    [NonSerialized] private BigInteger _money;

    public BigInteger Money
    {
        get => _money;
        set
        {
            _money = value;
            moneyStr = value.ToString();
        }
    }

    /// <summary>从 JSON 反序列化后调用，恢复运行时字段</summary>
    public void PostDeserialize()
    {
        if (!BigInteger.TryParse(moneyStr, out _money))
            _money = BigInteger.Zero;
    }

    // ── 材料操作 ───────────────────────────────────────────────────────────────

    public int GetItemCount(int itemId)
    {
        var entry = items.Find(e => e.itemId == itemId);
        return entry?.count ?? 0;
    }

    public void SetItemCount(int itemId, int count)
    {
        var entry = items.Find(e => e.itemId == itemId);
        if (entry != null)
            entry.count = Mathf.Max(0, count);
        else if (count > 0)
            items.Add(new ItemEntry { itemId = itemId, count = count });
    }

    public void AddItem(int itemId, int amount)
    {
        SetItemCount(itemId, GetItemCount(itemId) + amount);
    }

    public bool ConsumeItem(int itemId, int amount)
    {
        int current = GetItemCount(itemId);
        if (current < amount) return false;
        SetItemCount(itemId, current - amount);
        return true;
    }

    public Dictionary<int, int> GetItemDictionary()
    {
        var dict = new Dictionary<int, int>();
        foreach (var entry in items)
            dict[entry.itemId] = entry.count;
        return dict;
    }

    // ── 工厂方法 ───────────────────────────────────────────────────────────────

    public static GlobalSaveData CreateDefault()
    {
        var data = new GlobalSaveData
        {
            playerName = "Player",
            saveTime   = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            randomSeed = new System.Random().Next(int.MinValue, int.MaxValue)
        };
        data.Money = BigInteger.Zero;
        return data;
    }
}

[Serializable]
public class ItemEntry
{
    public int itemId;
    public int count;
}
