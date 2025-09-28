using System;
using UnityEngine;

public class UIManager : SingletonCom<UIManager>
{
    public Transform gameLayer;
    public LoadingCtrl loading;
    public GameConsole console;
    protected override void Awake()
    {
        base.Awake();
        console.Init();
        CoroutineTracker.BindLogFunc(str => console.AddLog(str, LogMsgType.Spec, new Color(0.85f, 1f, 0.68f, 1)));
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