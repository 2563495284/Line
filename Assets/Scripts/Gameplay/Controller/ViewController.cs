
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 提示类型
/// </summary>
public enum TipsType
{
    Info,       // 信息
    Success,    // 成功
    Warning,    // 警告
    Error       // 错误
}
public class ViewController : ControllBase
{
    private List<LevelView> views = new();
    private Dictionary<string, Func<object, IEnumerator>> performMap = new();
    public ViewController(LevelRoot viewRoot) : base(viewRoot)
    {
    }
    public void AddView(LevelView view)
    {
        views.Add(view);
        view.OnEnter();
    }
    public override void OnExit()
    {
        base.OnExit();
        views.ForEach(e => e.OnExit());
        views.Clear();
    }
    public IEnumerator RequestPerform(string key, object args)
    {
        if (performMap.ContainsKey(key))
            yield return performMap[key].Invoke(args);
        else
            yield return null;
    }
    public void BindPerform(string key, Func<object, IEnumerator> method)
    {
        if (performMap.ContainsKey(key))
            performMap[key] = method;
        else
            performMap.Add(key, method);
    }
    public void UnbindPerform(string key, Func<object, IEnumerator> method)
    {
        if (performMap[key] == method)
            performMap.Remove(key);
    }
    public T GetView<T>()
    {
        return default;
    }
}