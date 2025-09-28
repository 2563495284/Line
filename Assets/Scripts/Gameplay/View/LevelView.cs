using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
public abstract class LevelView : MonoBehaviour
{
    protected LevelModel Data => GM.Ins.Level.model;
    protected LevelConfig Cfg => GM.Ins.Level.Cfg;
    protected ViewController Ctrl => GM.Ins.View;
    private Dictionary<string, ECListener> wrappedMethods = new();
    private Dictionary<string, Func<object, IEnumerator>> wrappedPerforms = new();
    [SerializeField] private bool originActived = true;
    public int canvasOrder = 0;
    public void AddCanvasCom(GameObject go)
    {
        Ctrl.AddCanvasCom(go, canvasOrder);
    }
    protected void GetRes<T>(string resKey) where T : UnityEngine.Object
    {
        LoadManager.Ins.GetRes<T>(ResPath.Level.Key, resKey);
    }
    protected void AddCMD(LevelCommand cmd)
    {
        GM.Ins.Level.AddCMD(cmd);
    }
    protected void ExeCMD(LevelCommand cmd)
    {
        GM.Ins.Level.ExeCMDAsync(cmd);
    }
    protected void Register(string key, Action act)
    {
        Ctrl.On(key, act);
    }
    protected void Register<T>(string key, Action<T> evt)
    {
        Ctrl.On(key, Wrapper(key, evt));
    }
    protected void Unregister(string key, Action act)
    {
        Ctrl.Off(key, act);
    }
    protected void Unregister<T>(string key, Action<T> evt)
    {
        Ctrl.Off(key, Wrapper(key, evt));
    }


    protected void Bind(string key, Func<IEnumerator> perform)
    {
        Ctrl.BindPerform(key, PerformWrapper(key, perform));
    }
    protected void Bind<T>(string key, Func<T, IEnumerator> perform)
    {
        Ctrl.BindPerform(key, PerformWrapper(key, perform));
    }
    protected void Unbind(string key, Func<IEnumerator> perform)
    {
        Ctrl.UnbindPerform(key, PerformWrapper(key, perform));
    }
    protected void Unbind<T>(string key, Func<T, IEnumerator> perform)
    {
        Ctrl.UnbindPerform(key, PerformWrapper(key, perform));
    }

    public void Init()
    {
        OnInit();
        if (originActived)
            Show();
        else
            Hide();
    }
    public void Show()
    {
        gameObject.SetActive(true);
        OnShow();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        OnHide();
    }
    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    protected virtual void OnInit()
    {

    }
    private Func<object, IEnumerator> PerformWrapper<T>(string key, Func<T, IEnumerator> perform)
    {
        if (wrappedPerforms.ContainsKey(key))
            return wrappedPerforms[key];
        Func<object, IEnumerator> wrappedMethod = (object args) => perform.Invoke((T)args);
        wrappedPerforms.Add(key, wrappedMethod);
        return wrappedMethod;

    }
    private Func<object, IEnumerator> PerformWrapper(string key, Func<IEnumerator> perform)
    {
        if (wrappedPerforms.ContainsKey(key))
            return wrappedPerforms[key];
        Func<object, IEnumerator> wrappedMethod = (args) => perform.Invoke();
        wrappedPerforms.Add(key, wrappedMethod);
        return wrappedMethod;

    }
    private ECListener Wrapper<T>(string key, Action<T> method)
    {
        if (wrappedMethods.ContainsKey(key))
            return wrappedMethods[key];
        ECListener wrappedMethod = (object args) => method.Invoke((T)args);
        wrappedMethods.Add(key, wrappedMethod);
        return wrappedMethod;
    }
}