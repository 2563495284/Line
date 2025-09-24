using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GM : Singleton<GM>
{
    public LevelController Level { get; private set; }
    public ViewController View { get; private set; }
    public static LevelModel LevelData => Ins.Level.model;
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
        Level = new LevelController(null, systemRoot);
        View = new ViewController(viewRoot);
        Level.OnEnter();
        View.OnEnter();
    }
    public void ExitLevel()
    {
        Level.OnExit();
        View.OnExit();
        Level = null;
        View = null;
    }
}
