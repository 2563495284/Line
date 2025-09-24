//Object Pool

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OP : SingletonCom<OP>
{
    public Transform Root => transform;


    private static GameObject CachePanel;

    private static Dictionary<string, Queue<GameObject>> m_Pool = new Dictionary<string, Queue<GameObject>>();

    private static Dictionary<GameObject, string> m_GoTag = new Dictionary<GameObject, string>();

    private static Dictionary<string, int> m_IDCnt = new Dictionary<string, int>();

    static OP()
    {
        OP.m_Pool = new Dictionary<string, Queue<GameObject>>();
        OP.m_GoTag = new Dictionary<GameObject, string>();
        OP.m_IDCnt = new Dictionary<string, int>();
        OP.CachePanel = null;
    }
    private static string GetTagByPrefab(GameObject prefab)
    {
        return prefab.GetInstanceID().ToString();
    }
    public static void ClearCachePool()
    {
        OP.m_Pool.Clear();
        OP.m_GoTag.Clear();
        OP.m_IDCnt.Clear();
        while (OP.Ins.Root.childCount > 0)
        {
            UnityEngine.Object.DestroyImmediate(OP.Ins.Root.GetChild(0).gameObject);
        }
    }
    public static void ClearPoolByGameObject(GameObject go, bool onlyPool = false)
    {
        string tag = m_GoTag.ContainsKey(go) ? m_GoTag[go] : GetTagByPrefab(go);
        List<GameObject> delList = new();
        m_GoTag.Keys.Where(e => m_GoTag[e] == tag && (!onlyPool || CachePanel != null && e.transform.parent == CachePanel.transform)).ToList().ForEach(e =>
        {
            RemoveInstanceMark(e);
            DestroyImmediate(e);
        });
        m_Pool.Remove(tag);
        m_IDCnt.Remove(tag);
    }

    public static void ReturnCacheGameObject(GameObject go)
    {
        if (OP.CachePanel == null)
        {
            OP.CachePanel = new GameObject();
            OP.CachePanel.name = "CachePanel";
            OP.CachePanel.transform.SetParent(OP.Ins.Root);
        }
        if (go == null)
        {
            return;
        }
        go.transform.SetParent(OP.CachePanel.transform);
        go.SetActive(false);
        if (OP.m_GoTag.ContainsKey(go))
        {
            string key = OP.m_GoTag[go];
            if (!OP.m_Pool.ContainsKey(key))
            {
                OP.m_Pool[key] = new Queue<GameObject>();
            }
            OP.m_Pool[key].Enqueue(go);
        }
    }

    public static GameObject RequestCacheGameObject(GameObject prefab, Transform parent = null)
    {
        string tag = GetTagByPrefab(prefab);
        GameObject gameObject = OP.GetFromPool(tag);
        int id = OP.GetID(tag);
        if (gameObject == null)
        {
            gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, parent);
            OP.MarkInstance(gameObject, tag);
        }
        else if (parent != null)
            gameObject.transform.SetParent(parent);
        gameObject.name = prefab.name + id.ToString();
        return gameObject;
    }

    private static int GetID(string tag)
    {
        if (OP.m_IDCnt.ContainsKey(tag))
        {
            Dictionary<string, int> idcnt = OP.m_IDCnt;
            int num = idcnt[tag];
            idcnt[tag] = num + 1;
        }
        else
        {
            OP.m_IDCnt.Add(tag, 0);
        }
        return OP.m_IDCnt[tag];
    }

    private static GameObject GetFromPool(string tag)
    {
        if (OP.m_Pool.ContainsKey(tag) && OP.m_Pool[tag].Count > 0)
        {
            GameObject gameObject = OP.m_Pool[tag].Dequeue();
            if (gameObject)
            {
                gameObject.SetActive(true);
                return gameObject;
            }
        }
        return null;
    }

    private static void MarkInstance(GameObject go, string tag)
    {
        OP.m_GoTag.Add(go, tag);
    }

    private static void RemoveInstanceMark(GameObject go)
    {
        if (OP.m_GoTag.ContainsKey(go))
        {
            OP.m_GoTag.Remove(go);
            return;
        }
        Debug.LogError("remove out mark error, gameObject has not been marked");
    }

}
public static class GOExt
{
    public static GameObject OPGet(this GameObject prefab, Transform parent = null)
    {
        Transform p = parent == null ? OP.Ins.Root : parent;
        GameObject go = OP.RequestCacheGameObject(prefab, p);
        return go;
    }
    public static void OPClear(this GameObject go)
    {
        OP.ClearPoolByGameObject(go);
    }

    public static void OPPush(this GameObject obj)
    {
        OP.ReturnCacheGameObject(obj);
    }
}
