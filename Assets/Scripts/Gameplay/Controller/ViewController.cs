
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
    public Canvas gameCanvas { get; private set; }
    private Dictionary<int, Transform> viewComRootInCanvas = new();
    public ViewController(LevelRoot viewRoot) : base(viewRoot)
    {
        gameCanvas = new GameObject("gameCanvas").AddComponent<Canvas>();
        gameCanvas.transform.SetParent(viewRoot.transform);
    }
    public IEnumerator RequestPerform(string key, object args)
    {
        if (performMap.ContainsKey(key))
            yield return performMap[key];
        else
            yield return null;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        List<string> viewPaths = ResPath.Level.AllPaths.Where(e => e.Contains("Level/View")).ToList();
        viewPaths.ForEach(e =>
        {
            LevelView view = LoadManager.Ins.GetRes<GameObject>("Level", e).OPGet().GetComponent<LevelView>();
            view.transform.SetParent(Root.transform);
            views.Add(view);
            view.Init();
        });
    }
    public override void OnExit()
    {
        base.OnExit();
        views.ForEach(e =>
        {
            e.Hide();
            e.gameObject.OPPush();
        });
        gameCanvas = null;
        GameObject.Destroy(Root.gameObject);
    }
    public void AddCanvasCom(GameObject com, int index)
    {
        if (!viewComRootInCanvas.ContainsKey(index))
        {
            RectTransform rt = new GameObject($"comRoot_{index}").AddComponent<RectTransform>();
            rt.SetParent(gameCanvas.transform);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(0, 0);
            viewComRootInCanvas.Add(index, rt);
            rt.SetSiblingIndex(index);
        }
        com.transform.SetParent(viewComRootInCanvas[index]);
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
    public void RegisterView(LevelView view)
    {
        views.Add(view);
    }
    public void UnregisterView(LevelView view)
    {
        views.Remove(view);
    }
    public T GetView<T>()
    {
        return default;
    }
    public void ShowTips(string message, TipsType tipsType = TipsType.Info, float duration = -1)
    {

    }
}