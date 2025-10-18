using System;
using System.Collections.Generic;

public static class RNG
{

    public static float Noise21(float x, float y)
    {
        Func<float, float> fract = (n) => n - MathF.Floor(n);
        return fract(MathF.Sin(x * 1738.9f + y * 8635.3f) * 3162.7f);
    }
    public static float Rand()
    {
        return Noise21(1, DateTime.Now.Millisecond % 17000000);
    }
    public static float Rand(float min, float max, Func<float, float>? func = null)
    {
        func ??= t => t;
        return func(Rand()) * (max - min) + min;
    }
    public static int RandI(int min, int max)
    {
        return Math.Min(max, (int)MathF.Floor(Rand(min, max + 1)));
    }
    public static T Rand<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            return default;
        return list[RandI(0, list.Count - 1)];
    }
    public static T WeiRand<T>(this List<T> element, List<float> wei)
    {
        if (wei.Count != element.Count)
        {
            return default;
        }
        float probCnt = 0;
        float rand = Rand();
        float allWei = 0;
        wei.ForEach(e => allWei += e);
        for (int i = 0; i < wei.Count; i++)
        {
            T e = element[i];
            float p = wei[i] / allWei;
            if (rand <= probCnt + p)
                return e;
            probCnt += p;
        }
        return element[element.Count - 1];
    }
    /**wei:权重数组，element:元素数组，isRepeat:是否放回 */
    public static List<T> WeiRandMul<T>(this List<T> element, List<float> wei, int m, bool isRepeat)
    {
        List<float> w = new(wei);
        List<T> e = new(element);
        List<T> res = new();
        for (int i = 0; i < m; i++)
        {
            if (w.Count == 0)
                return res;
            T r = WeiRand(e, w);
            if (!isRepeat)
            {
                int idx = e.IndexOf(r);
                w.RemoveAt(idx);
                e.RemoveAt(idx);
            }
            res.Add(r);
        }
        return res;
    }
}