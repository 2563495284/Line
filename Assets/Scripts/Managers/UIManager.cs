using System;
using UnityEngine;

public class UIManager : SingletonCom<UIManager>
{
    public Transform gameLayer;
    public RectTransform gameUILayer;
    public RectTransform UILayer;
    public LoadingCtrl loading;
    public void LoadingUI(string key, Action onFinish = null)
    {
        loading.Show();
        LoadManager.Ins.LoadAsync(key, UpdateLoading, (LoadCompleteEventArgs args) =>
        {
            loading.Hide();
            onFinish?.Invoke();
        });
    }
    public void UpdateLoading(LoadProgressEventArgs args)
    {
        loading.SetProgress(args.Progress);
        loading.SetTips($"正在加载资源{args.LoadedCount}/{args.TotalCount}");
    }
}