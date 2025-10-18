using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonCom<UIManager>
{
    public Transform gameLayer;
    public Transform uiLayer;
    public LoadingCtrl loading;
    public GameConsole console;
    private static Dictionary<string, GameObject> uiMap = new();
    public static void BindUI(string key, GameObject root)
    {
        uiMap.Add(key, root);
    }
    public static void DestoryUI(string key)
    {
        if (uiMap.ContainsKey(key))
        {
            Destroy(uiMap[key]);
            uiMap.Remove(key);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        console.Init();
        CoroutineTracker.BindLogFunc(str => console.AddLog(str, LogMsgType.Start),
            str => console.AddLog(str, LogMsgType.End));
    }
    public void LoadingUI(string key, Action onFinish = null)
    {
        loading.Show();
        StartCoroutine(LoadManager.Ins.LoadAsync(key, UpdateLoading, (LoadCompleteEventArgs args) =>
        {
            loading.Hide();
            onFinish?.Invoke();
        }));
    }
    public void UpdateLoading(LoadProgressEventArgs args)
    {
        loading.SetProgress(args.Progress);
        loading.SetTips($"正在加载资源{args.LoadedCount}/{args.TotalCount}");
    }
    void Update()
    {
        bool isConsoleActive = console.gameObject.activeSelf;
        if (Input.GetKeyDown(KeyCode.Period))
        {
            if (isConsoleActive)
                console.Hide();
            else
                console.Show();
        }
        if (isConsoleActive && Input.GetKeyDown(KeyCode.Escape))
            console.Hide();
    }
}