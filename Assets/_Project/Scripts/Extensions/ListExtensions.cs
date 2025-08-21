using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// 从列表顶部抽取一个元素（移除并返回第一个元素）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="list">目标列表</param>
    /// <returns>抽取的元素，如果列表为空则返回默认值</returns>
    public static T Draw<T>(this List<T> list)
    {
        if (list.Count == 0) return default;

        T item = list[0];
        list.RemoveAt(0);
        return item;
    }

    /// <summary>
    /// 随机抽取一个元素（移除并返回随机位置的元素）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="list">目标列表</param>
    /// <returns>抽取的元素，如果列表为空则返回默认值</returns>
    public static T DrawRandom<T>(this List<T> list)
    {
        if (list.Count == 0) return default;

        int randomIndex = Random.Range(0, list.Count);
        T item = list[randomIndex];
        list.RemoveAt(randomIndex);
        return item;
    }

    /// <summary>
    /// 查看列表顶部的元素（不移除）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="list">目标列表</param>
    /// <returns>顶部元素，如果列表为空则返回默认值</returns>
    public static T Peek<T>(this List<T> list)
    {
        if (list.Count == 0) return default;
        return list[0];
    }

    /// <summary>
    /// 随机打乱列表顺序（Fisher-Yates算法）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="list">目标列表</param>
    public static void Shuffle<T>(this List<T> list)
    {
        if (list.Count <= 1) return;

        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // 交换元素位置
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 随机获取一个元素（不移除）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="list">目标列表</param>
    /// <returns>随机元素，如果列表为空则返回默认值</returns>
    public static T RandomElement<T>(this List<T> list)
    {
        if (list.Count == 0) return default;
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// 随机获取一个元素（不移除）- 支持IEnumerable
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="enumerable">目标集合</param>
    /// <returns>随机元素，如果集合为空则返回默认值</returns>
    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
        var list = enumerable.ToList();
        return list.RandomElement();
    }

    /// <summary>
    /// 随机获取多个元素（不移除）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="list">目标列表</param>
    /// <param name="count">要获取的元素数量</param>
    /// <returns>随机元素列表</returns>
    public static List<T> RandomElements<T>(this List<T> list, int count)
    {
        if (list.Count == 0 || count <= 0) return new List<T>();

        count = Mathf.Min(count, list.Count);
        var result = new List<T>();
        var indices = new HashSet<int>();

        while (result.Count < count)
        {
            int randomIndex = Random.Range(0, list.Count);
            if (indices.Add(randomIndex))
            {
                result.Add(list[randomIndex]);
            }
        }

        return result;
    }

    /// <summary>
    /// 根据权重随机选择元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="list">目标列表</param>
    /// <param name="weights">权重列表</param>
    /// <returns>根据权重随机选择的元素</returns>
    public static T RandomElementWeighted<T>(this List<T> list, List<float> weights)
    {
        if (list.Count == 0 || weights.Count != list.Count) return default;

        float totalWeight = weights.Sum();
        if (totalWeight <= 0) return list.RandomElement();

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        for (int i = 0; i < list.Count; i++)
        {
            currentWeight += weights[i];
            if (randomValue <= currentWeight)
            {
                return list[i];
            }
        }

        return list[list.Count - 1]; // 后备选择
    }

    /// <summary>
    /// 随机获取一个索引
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="list">目标列表</param>
    /// <returns>随机索引，如果列表为空则返回-1</returns>
    public static int RandomIndex<T>(this List<T> list)
    {
        if (list.Count == 0) return -1;
        return Random.Range(0, list.Count);
    }
}