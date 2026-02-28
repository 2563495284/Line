using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局随机管理器。
///
/// 所有游戏内随机取值必须通过此类，禁止直接调用 UnityEngine.Random 或 new System.Random()。
/// 种子来自存档 (GlobalSaveData.randomSeed)，保证同一存档可复现随机序列。
///
/// 使用示例：
///   int   n = RandomManager.Instance.NextInt(1, 100);
///   float f = RandomManager.Instance.NextFloat();
///   var   list = RandomManager.Instance.Shuffle(myList);
/// </summary>
public class RandomManager : PersistentSingleton<RandomManager>
{
    private System.Random _rng;

    public int Seed { get; private set; }
    public bool IsInitialized { get; private set; }

    // ── 初始化 ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// 用存档种子初始化。由 GameFlowManager 在新游戏/读档后立即调用。
    /// </summary>
    public void Initialize(int seed)
    {
        Seed = seed;
        _rng = new System.Random(seed);
        IsInitialized = true;

        // 同步 UnityEngine.Random，保证 Unity 内置 API 也受控
        UnityEngine.Random.InitState(seed);

        Debug.Log($"[RandomManager] 已初始化，种子: {seed}");
    }

    // ── 整数 ───────────────────────────────────────────────────────────────────

    /// <summary>返回 [0, int.MaxValue) 的随机整数</summary>
    public int NextInt() => Rng().Next();

    /// <summary>返回 [0, maxExclusive) 的随机整数</summary>
    public int NextInt(int maxExclusive) => Rng().Next(maxExclusive);

    /// <summary>返回 [min, maxExclusive) 的随机整数</summary>
    public int NextInt(int min, int maxExclusive) => Rng().Next(min, maxExclusive);

    // ── 浮点 ───────────────────────────────────────────────────────────────────

    /// <summary>返回 [0.0, 1.0) 的随机 float</summary>
    public float NextFloat() => (float)Rng().NextDouble();

    /// <summary>返回 [min, max) 的随机 float</summary>
    public float NextFloat(float min, float max) => min + (float)Rng().NextDouble() * (max - min);

    /// <summary>返回 [0.0, 1.0) 的随机 double</summary>
    public double NextDouble() => Rng().NextDouble();

    // ── 布尔 ───────────────────────────────────────────────────────────────────

    /// <summary>50% 概率返回 true</summary>
    public bool NextBool() => Rng().Next(2) == 0;

    /// <summary>按给定概率返回 true，probability 范围 [0, 1]</summary>
    public bool NextChance(float probability) => (float)Rng().NextDouble() < probability;

    // ── 集合 ───────────────────────────────────────────────────────────────────

    /// <summary>Fisher-Yates 原地打乱，返回同一个 list</summary>
    public List<T> Shuffle<T>(List<T> list)
    {
        var rng = Rng();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

    /// <summary>从列表中随机取一个元素</summary>
    public T Pick<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("[RandomManager] Pick: 列表为空");
        return list[NextInt(list.Count)];
    }

    /// <summary>从数组中随机取一个元素</summary>
    public T Pick<T>(T[] array)
    {
        if (array == null || array.Length == 0)
            throw new ArgumentException("[RandomManager] Pick: 数组为空");
        return array[NextInt(array.Length)];
    }

    // ── 私有工具 ───────────────────────────────────────────────────────────────

    private System.Random Rng()
    {
        if (!IsInitialized)
        {
            Debug.LogWarning("[RandomManager] 尚未初始化，使用临时种子 0。请在游戏启动时调用 Initialize()");
            Initialize(0);
        }
        return _rng;
    }
}
