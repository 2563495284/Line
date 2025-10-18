using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum Dir8
{
    Up = 0,
    Right,
    Down,
    Left,
    RU,
    RD,
    LD,
    LU
}
public enum Dir4
{
    Up = 0,
    Right,
    Down,
    Left,
}
public static class MUtils
{
    public static Dir8 Toward(this Vector2 vector)
    {
        float angle = Vector2.SignedAngle(Vector2.right, vector);
        float curAngle = -157.5f;
        for (int i = 0; i <= 8; i++)
        {
            if (angle < curAngle + i * 45f)
            {
                switch (i)
                {
                    case 0:
                    case 8:
                        return Dir8.Left;
                    case 1:
                        return Dir8.LD;
                    case 2:
                        return Dir8.Down;
                    case 3:
                        return Dir8.RD;
                    case 4:
                        return Dir8.Right;
                    case 5:
                        return Dir8.RU;
                    case 6:
                        return Dir8.Up;
                    case 7:
                        return Dir8.LU;
                }
            }
        }
        return Dir8.Right;
    }
    public static Vector2 screenSize => new Vector2(Screen.width, Screen.height);
    public static void SetImgRatio(Image img)
    {
        if (img.sprite == null)
            return;
        float ratio = (float)img.mainTexture.width / img.mainTexture.height;
        float hei = img.rectTransform.sizeDelta.y;
        img.rectTransform.sizeDelta = new Vector2(ratio * hei, hei);
    }
    public static Color ParseColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var color);
        return color;
    }
    public static void SetSpriteRatio(SpriteRenderer img)
    {
        if (img.sprite == null)
            return;
        float ratio = (float)img.sprite.texture.width / img.sprite.texture.height;
        img.transform.localScale = new Vector3(ratio, 1, 1);
    }

    /// <summary>
    /// 判断对象是否实现了IEnumerable或IEnumerable<T>
    /// </summary>
    /// <param name="obj">要检查的对象</param>
    /// <returns>如果是IEnumerable类型返回true，否则返回false</returns>
    public static bool IsEnumerable(this Type type)
    {
        if (type == null)
            return false;

        // 检查是否直接实现了非泛型IEnumerable
        if (typeof(IEnumerable).IsAssignableFrom(type))
            return true;

        // 检查是否实现了泛型IEnumerable<T>
        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return true;
            }
        }

        return false;
    }

    ///<summary> 输出target在self坐标系下的锚点位置
    public static Vector2 TransAnchorPos(this RectTransform self, RectTransform target)
    {
        Vector2 vector = target.anchoredPosition;
        if (target.parent)
        {
            vector = target.parent.TransformPoint(target.anchoredPosition);
        }
        if (self.parent)
        {
            return self.parent.InverseTransformPoint(vector);
        }
        return vector;
    }
    public static Vector2 V2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
    public static Vector3 Rotate(this Vector3 v, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * v;
    }

    public static Vector3 V3(this Vector2 v, float z = 0f)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static float Ang(this Vector2 v)
    {
        return Vector2.SignedAngle(Vector2.right, v);
    }

    public static bool ValidIdx(this Array array, int idx)
    {
        if (idx < 0 || idx >= array.Length)
            return false;
        return true;
    }
    public static bool ValidIdx<T>(this List<T> list, int idx)
    {
        if (idx < 0 || idx >= list.Count)
            return false;
        return true;
    }
    public static T GetRandom<T>(this T[] list)
    {
        return list[UnityEngine.Random.Range(0, list.Length)];
    }
    public static float RandF()
    {
        return UnityEngine.Random.Range(0, 1);
    }
    public static int Rand(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public static float RandF(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public static T GetRandom<T>(this List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    public static float Remap(float oldMin, float oldMax, float newMin, float newMax, float value)
    {
        return (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
    }
    public static float RemapClamp(float oldMin, float oldMax, float newMin, float newMax, float value)
    {
        return Math.Clamp(Remap(oldMin, oldMax, newMin, newMax, value), newMin, newMax);
    }
    public static int Sum<T>(this T[] array, Func<T, int> func)
    {
        int res = 0;
        for (int i = 0; i < array.Length; i++)
        {
            res += func(array[i]);
        }
        return res;
    }
    public static int Sum<T>(this List<T> array, Func<T, int> func)
    {
        int res = 0;
        for (int i = 0; i < array.Count; i++)
        {
            res += func(array[i]);
        }
        return res;
    }
    public static string GetGoPath(this Transform root, GameObject target)
    {
        if (root == null || target == null)
            return "#Error#";
        string path = "";
        Transform t = target.transform;
        while (t && t != root)
        {
            path = t.name + "/" + path;
            t = t.parent;
        }
        if (t == null)
            return "#Error#";
        else
            return path;
    }
    public static GameObject GetGoByPath(this Transform root, string path)
    {
        if (root == null)
            return null;
        Transform t = root.Find(path);
        if (t == null)
            return null;
        else
            return t.gameObject;
    }
    public static T GetCom<T>(this Transform root, string path = "") where T : Component
    {
        GameObject go = root.GetGoByPath(path);
        if (go == null)
            return null;
        else
            return go.GetComponent<T>();
    }
    public static Vector3 GetMouseWp(float zValue = 0f)
    {
        Plane dragPlane = new(Camera.main.transform.forward, new Vector3(0, 0, zValue));
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (dragPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
    public static bool Contains<T>(this T[] array, T e)
    {
        if (array == null)
            return false;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(e))
                return true;
        }
        return false;
    }
    public static void Fill<K, V>(this Dictionary<K, V> d, K key, V val)
    {
        if (d.ContainsKey(key))
            d[key] = val;
        else
            d.Add(key, val);
    }
    public static Dictionary<K, V> ToMap<K, V>(this V[] src, Func<V, K> func)
    {
        Dictionary<K, V> dic = new Dictionary<K, V>();
        for (int i = 0; i < src.Length; i++)
        {
            dic.Add(func(src[i]), src[i]);
        }
        return dic;
    }
    public static Dictionary<K, V> ToMap<K, V>(this List<V> src, Func<V, K> func)
    {
        return src.ToArray().ToMap(func);
    }
    public static Dictionary<K, V> ToMap<D, K, V>(this List<D> src, Func<D, K> funcK, Func<D, V> funcV)
    {
        Dictionary<K, V> dic = new Dictionary<K, V>();
        for (int i = 0; i < src.Count; i++)
        {
            dic.Add(funcK(src[i]), funcV(src[i]));
        }
        return dic;
    }
    ///<summary> 分析上一次与当前的数组变化，返回(enter,exit) </summary>
    public static (List<T>, List<T>) Analyse<T>(T[] last, T[] now)
    {
        List<T> enters = new List<T>();
        List<T> exits = new List<T>();
        if (last != null)
            for (int i = 0; i < last.Length; i++)
            {
                if (now == null || !now.Contains(last[i]))
                    exits.Add(last[i]);
            }
        if (now != null)
            for (int i = 0; i < now.Length; i++)
            {
                if (last == null || !last.Contains(now[i]))
                    enters.Add(now[i]);
            }
        return (enters, exits);
    }
    public static (List<T>, List<T>) Analyse<T>(List<T> last, List<T> now)
    {
        List<T> enters = new List<T>();
        List<T> exits = new List<T>();
        if (last != null)
            for (int i = 0; i < last.Count; i++)
            {
                if (now == null || !now.Contains(last[i]))
                    exits.Add(last[i]);
            }
        if (now != null)
            for (int i = 0; i < now.Count; i++)
            {
                if (last == null || !last.Contains(now[i]))
                    enters.Add(now[i]);
            }
        return (enters, exits);
    }
    public static string FormatNumber(int num)
    {
        // 处理负数情况
        if (num < 0)
        {
            return "-" + FormatNumber(-num);
        }

        // 十亿级别 (1,000,000,000+)
        if (num >= 1000000000)
        {
            float value = num / 1000000000f;
            return value.ToString("F2") + "B";
        }

        // 百万级别 (1,000,000+)
        if (num >= 1000000)
        {
            float value = num / 1000000f;
            return value.ToString("F2") + "M";
        }

        // 千级别 (1,000+)
        if (num >= 1000)
        {
            float value = num / 1000f;
            return value.ToString("F2") + "k";
        }

        // 小于1000的数字直接返回
        return num.ToString();
    }
    public static (T, T) Analyse<T>(T last, T now)
    {
        T enter = default(T);
        T exit = default(T);
        if (last == null && now != null)
            enter = now;
        else if (last != null && now == null)
            exit = last;
        else if (last != null && now != null && !last.Equals(now))
        {
            enter = now;
            exit = last;
        }
        return (enter, exit);
    }
    /// <summary>
    /// 保留n位有效数字
    /// </summary>
    /// <param name="value"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static float RoundTo(this float value, int n)
    {
        if (value == 0) return 0;

        float factor = (float)Math.Pow(10, n - (int)Math.Floor(Math.Log10(Math.Abs(value))) - 1);
        return (float)Math.Round(value * factor) / factor;
    }
    public static K RecurFind<T, K>(this T obj, Func<T, T> getParent, Func<T, K> func) where T : class where K : class
    {
        while (obj != null)
        {
            K res = func(obj);
            if (res != null)
                return res;
            obj = getParent(obj);
        }
        return null;
    }
    public static K RecurFind<T, K>(this T obj, Func<T, T> getParent, Func<T, bool> predicate, Func<T, K> func) where T : class where K : class
    {
        while (obj != null)
        {
            if (predicate(obj))
                return func(obj);
            obj = getParent(obj);
        }
        return null;
    }
    public static void Recursive<T>(this T obj, Func<T, T> getNext, Action<T> func) where T : class
    {
        while (obj != null)
        {
            func(obj);
            obj = getNext(obj);
        }
    }
    public static bool RecurJudge<T>(this T obj, Func<T, T> getParent, Func<T, bool> func) where T : class
    {

        bool res = false;
        while (obj != null)
        {
            res = func(obj);
            if (res != false)
                break;
            obj = getParent(obj);
        }
        return res;
    }
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/AlignAnchorToRect")]
    //rectTransform有缩放则会拉伸图片！
    private static void AlignAnchorToRect()
    {
        GameObject[] gos = UnityEditor.Selection.gameObjects;
        for (int i = 0; i < gos.Length; i++)
        {
            RectTransform rt = (gos[i].transform as RectTransform);
            RectTransform pt = rt.parent as RectTransform;
            Vector2 size = rt.sizeDelta;
            Vector2 ptSize = new Vector2(pt.rect.width, pt.rect.height);
            Vector2 ptMin = new Vector2(pt.position.x, pt.position.y) - ptSize / 2;
            Vector3 pos = rt.transform.position;
            rt.anchorMin = (new Vector2(rt.position.x, rt.position.y) - new Vector2(rt.rect.width, rt.rect.height) / 2 - ptMin) / ptSize;
            rt.anchorMax = (new Vector2(rt.position.x, rt.position.y) + new Vector2(rt.rect.width, rt.rect.height) / 2 - ptMin) / ptSize;
            rt.sizeDelta = Vector2.zero;
            rt.transform.position = pos;
        }
    }


    [UnityEditor.MenuItem("Tools/AlignAnchorToPivot")]
    private static void AlignAnchorToPivot()
    {
        GameObject[] gos = UnityEditor.Selection.gameObjects;
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i].GetComponent<RectTransform>() == null)
                continue;
            AdaptPivot(gos[i]);
        }
    }

    private static void AdaptPivot(GameObject go)
    {
        //------获取rectTransform----
        RectTransform partentRect = go.transform.parent.GetComponent<RectTransform>();
        RectTransform localRect = go.GetComponent<RectTransform>();

        //位置信息
        Vector3 partentPos = go.transform.parent.position;
        Vector3 localPos = go.transform.position;

        float partentWidth = partentRect.rect.width;
        float partentHeight = partentRect.rect.height;

        //---------位移差------
        float offX = localPos.x - partentPos.x;
        float offY = localPos.y - partentPos.y;

        float rateW = offX / partentWidth;
        float rateH = offY / partentHeight;
        var anchor = new Vector2(.5f + rateW, .5f + rateH);
        localRect.SetRtAnchorSafe(anchor, anchor);
    }


    public static void SetRtAnchorSafe(this RectTransform rt, Vector2 anchorMin, Vector2 anchorMax)
    {
        if (anchorMin.x < 0 || anchorMin.x > 1 || anchorMin.y < 0 || anchorMin.y > 1 || anchorMax.x < 0 || anchorMax.x > 1 || anchorMax.y < 0 || anchorMax.y > 1)
            return;

        var lp = rt.localPosition;
        //注意不要直接用sizeDelta因为该值会随着anchor改变而改变
        var ls = new Vector2(rt.rect.width, rt.rect.height);

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;

        //动态改变anchor后size和localPostion可能会发生变化需要重新设置
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ls.x);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ls.y);
        rt.localPosition = lp;
    }

    public static Vector2 GetWorldPosition()
    {
        UnityEditor.SceneView sceneView = UnityEditor.SceneView.currentDrawingSceneView;
        Camera cam = sceneView.camera;
        Vector2 mousePos = Event.current.mousePosition;
        mousePos.x *= 1.25f;
        mousePos.y = cam.pixelHeight - mousePos.y * 1.25f;
        mousePos = sceneView.camera.ScreenToWorldPoint(mousePos);
        return mousePos;
    }

#endif
    public static void SetFullRect(this GameObject ui)
    {
        if (ui.transform is RectTransform rt)
        {
            rt.SetRtAnchorSafe(Vector2.zero, Vector2.one);
            rt.anchoredPosition = new Vector2(0, 0);
            rt.sizeDelta = new Vector2(0, 0);
        }
    }
    public static int Mod(int x, int k)
    {
        while (x < 0)
            x += k;
        return x % k;
    }
    public static float RLerp(float a, float b, float x)
    {
        return (x - a) / (b - a);
    }
    public static string Hash(this System.Object o, string prefix)
    {
        return prefix + o.GetHashCode().ToString();
    }
    public static int ChessDist(Vector3Int a, Vector3Int b)
    {
        return Mathf.Max(Mathf.Abs(b.x - a.x), Mathf.Abs(b.y - a.y));
    }
}
