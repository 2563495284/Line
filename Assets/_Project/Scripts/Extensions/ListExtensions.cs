using System.Collections.Generic;
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
}