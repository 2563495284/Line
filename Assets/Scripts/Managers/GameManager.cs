using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GM : Singleton<GM>
{
    public LevelController Level { get; private set; }
    public ViewController View { get; private set; }
    public static LevelModel LevelData => Ins.Level.model;

    public bool IsTrackCoroutine => UIManager.Ins.console.isTrackCoroutine;
    public void EnterGame()
    {
        UIManager.Ins.LoadingUI("Level", () =>
        {
            LevelRoot systemRoot = new GameObject("SystemRoot").AddComponent<LevelRoot>();
            systemRoot.transform.SetParent(UIManager.Ins.gameLayer);
            LevelRoot viewRoot = new GameObject("LevelViews").AddComponent<LevelRoot>();
            viewRoot.transform.SetParent(UIManager.Ins.gameLayer);
            StartLevel(systemRoot, viewRoot);
        });
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public static void Tips(string message, TipsType tipsType = TipsType.Info, float duration = -1)
    {
        Ins.View.ShowTips(message, tipsType, duration);
    }
    public void StartLevel(LevelRoot systemRoot, LevelRoot viewRoot)
    {
        LevelConfig commonCfg = LoadManager.Ins.GetRes<LevelConfig>("Level", ResPath.Level.CommonLevel);
        Level = new LevelController(commonCfg, systemRoot);
        View = new ViewController(viewRoot);
        Level.OnEnter();
        View.OnEnter();
        Level.OnStart();
    }
    public void ExitLevel()
    {
        Level.OnExit();
        View.OnExit();
        Level = null;
        View = null;
    }
    public void OnUpdate()
    {

    }
}
