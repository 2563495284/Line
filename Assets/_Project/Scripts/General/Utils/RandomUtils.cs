using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 随机工具类 - 提供各种随机功能
/// </summary>
public static class RandomUtils
{
    /// <summary>
    /// 随机布尔值
    /// </summary>
    /// <param name="probability">为true的概率（0-1）</param>
    /// <returns>随机布尔值</returns>
    public static bool RandomBool(float probability = 0.5f)
    {
        return Random.Range(0f, 1f) < probability;
    }

    /// <summary>
    /// 随机符号（-1或1）
    /// </summary>
    /// <returns>-1或1</returns>
    public static int RandomSign()
    {
        return Random.Range(0, 2) == 0 ? -1 : 1;
    }

    /// <summary>
    /// 在范围内随机浮点数（包含最小值和最大值）
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机浮点数</returns>
    public static float RandomFloat(float min, float max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// 在范围内随机整数（包含最小值，不包含最大值）
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值（不包含）</param>
    /// <returns>随机整数</returns>
    public static int RandomInt(int min, int max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// 随机颜色
    /// </summary>
    /// <param name="includeAlpha">是否包含透明度</param>
    /// <returns>随机颜色</returns>
    public static Color RandomColor(bool includeAlpha = false)
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        float a = includeAlpha ? Random.Range(0f, 1f) : 1f;
        return new Color(r, g, b, a);
    }

    /// <summary>
    /// 随机Vector2
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机Vector2</returns>
    public static Vector2 RandomVector2(Vector2 min, Vector2 max)
    {
        return new Vector2(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y)
        );
    }

    /// <summary>
    /// 随机Vector3
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机Vector3</returns>
    public static Vector3 RandomVector3(Vector3 min, Vector3 max)
    {
        return new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );
    }

    /// <summary>
    /// 随机单位圆内的点
    /// </summary>
    /// <returns>单位圆内的随机点</returns>
    public static Vector2 RandomPointInCircle()
    {
        return Random.insideUnitCircle;
    }

    /// <summary>
    /// 随机单位圆上的点
    /// </summary>
    /// <returns>单位圆上的随机点</returns>
    public static Vector2 RandomPointOnCircle()
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    /// <summary>
    /// 随机单位球内的点
    /// </summary>
    /// <returns>单位球内的随机点</returns>
    public static Vector3 RandomPointInSphere()
    {
        return Random.insideUnitSphere;
    }

    /// <summary>
    /// 随机单位球面上的点
    /// </summary>
    /// <returns>单位球面上的随机点</returns>
    public static Vector3 RandomPointOnSphere()
    {
        return Random.onUnitSphere;
    }

    /// <summary>
    /// 随机旋转
    /// </summary>
    /// <returns>随机旋转</returns>
    public static Quaternion RandomRotation()
    {
        return Random.rotation;
    }

    /// <summary>
    /// 随机旋转（仅绕Z轴）
    /// </summary>
    /// <returns>绕Z轴的随机旋转</returns>
    public static Quaternion RandomRotationZ()
    {
        return Quaternion.Euler(0, 0, Random.Range(0f, 360f));
    }

    /// <summary>
    /// 根据概率表随机选择索引
    /// </summary>
    /// <param name="probabilities">概率数组</param>
    /// <returns>选中的索引，如果数组为空或概率和为0则返回-1</returns>
    public static int RandomIndexByProbability(float[] probabilities)
    {
        if (probabilities == null || probabilities.Length == 0) return -1;

        float totalProbability = 0f;
        foreach (float prob in probabilities)
        {
            totalProbability += prob;
        }

        if (totalProbability <= 0f) return -1;

        float randomValue = Random.Range(0f, totalProbability);
        float currentProbability = 0f;

        for (int i = 0; i < probabilities.Length; i++)
        {
            currentProbability += probabilities[i];
            if (randomValue <= currentProbability)
            {
                return i;
            }
        }

        return probabilities.Length - 1; // 后备选择
    }

    /// <summary>
    /// 随机字符串
    /// </summary>
    /// <param name="length">字符串长度</param>
    /// <param name="characters">可选字符集</param>
    /// <returns>随机字符串</returns>
    public static string RandomString(int length, string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
    {
        if (length <= 0 || string.IsNullOrEmpty(characters)) return string.Empty;

        System.Text.StringBuilder result = new System.Text.StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            result.Append(characters[Random.Range(0, characters.Length)]);
        }
        return result.ToString();
    }

    /// <summary>
    /// 随机GUID字符串
    /// </summary>
    /// <returns>随机GUID字符串</returns>
    public static string RandomGuid()
    {
        return System.Guid.NewGuid().ToString();
    }
}
