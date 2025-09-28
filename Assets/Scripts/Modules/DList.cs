using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IIndexableElement<T>
{
    T GetKey();
}
public class DList<T, K> : IEnumerable<K> where K : IIndexableElement<T>
{
    private List<K> list = new();
    private Dictionary<T, K> dict = new();
    public DList()
    {
        list = new();
        dict = new();
    }
    public DList(IEnumerable<K> iterator)
    {
        list = new();
        dict = new();
        foreach (K e in iterator)
        {
            Add(e);
        }
    }
    public IEnumerable<T> Keys => dict.Keys;
    public IEnumerable<K> Elements => list;
    public void Add(K element)
    {
        list.Add(element);
        dict.Add(element.GetKey(), element);
    }
    public K Peek()
    {
        return list[^1];
    }
    public int Count => list.Count;
    public K Dequeue()
    {
        K element = list[0];
        dict.Remove(element.GetKey());
        list.RemoveAt(0);
        return element;
    }
    public void Enqueue(K element)
    {
        Add(element);
    }
    public void Push(K element)
    {
        Add(element);
    }
    public K Pop()
    {
        int cnt = Count;
        K element = list[cnt - 1];
        dict.Remove(element.GetKey());
        list.RemoveAt(cnt - 1);
        return element;
    }
    public void Remove(K element)
    {
        list.Remove(element);
        dict.Remove(element.GetKey());
    }
    public K this[T key]
    {
        get
        {
            if (!dict.ContainsKey(key))
                return default;
            return dict[key];
        }
    }
    public bool Has(T key)
    {
        return dict.ContainsKey(key);
    }
    public bool Has(K element)
    {
        return Has(element.GetKey());
    }
    public void Clear()
    {
        list.Clear();
        dict.Clear();
    }

    public IEnumerator<K> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public List<K> List => list;

}
