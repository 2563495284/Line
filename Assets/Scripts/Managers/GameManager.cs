using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GM : Singleton<GM>
{
    public LevelController Level { get; private set; }
    public static LevelModel LevelData => Ins.Level.model;

    public bool IsTrackCoroutine => UIManager.Ins.console.isTrackCoroutine;
    public static void AddLog(string logMsg, LogMsgType type, Color? specColor = null)
    {
        UIManager.Ins.console.AddLog(logMsg, type, specColor);
    }
    public void EnterGame()
    {
        UIManager.Ins.LoadingUI("Start", () =>
        {
            GameObject root = GameObject.Instantiate(LoadManager.Ins.GetRes<GameObject>("Start", ResPath.Start.StartUI));
            root.transform.SetParent(UIManager.Ins.uiLayer);
            root.SetFullRect();
            UIManager.BindUI("Start", root);
        });
    }
    public void StartLevel()
    {
        AddLog("Load Level Resource", LogMsgType.Start);
        UIManager.Ins.LoadingUI("Level", () =>
        {
            UIManager.DestoryUI("Start");
            GameObject gameRoot = new GameObject("LevelRoot");
            LevelRoot levelRoot = gameRoot.AddComponent<LevelRoot>();
            gameRoot.transform.SetParent(UIManager.Ins.gameLayer);
            Level = new LevelController(levelRoot, LoadManager.Ins.GetRes<LevelConfig>(ResPath.Level.Key, ResPath.Level.LevelConfig));
            GameObject root = GameObject.Instantiate(LoadManager.Ins.GetRes<GameObject>(ResPath.Level.Key, ResPath.Level.GameRoot));
            root.transform.SetParent(gameRoot.transform);
            UIManager.BindUI("Level", gameRoot);
            Level.OnEnter();
            AddLog("Load Level Resource", LogMsgType.End);
        });

    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ExitLevel()
    {
        Level.OnExit();
        Level = null;
        UIManager.Ins.LoadingUI("Start", () =>
        {
            UIManager.DestoryUI("Level");
            GameObject root = GameObject.Instantiate(LoadManager.Ins.GetRes<GameObject>(ResPath.Start.Key, ResPath.Start.StartUI));
            root.transform.SetParent(UIManager.Ins.uiLayer);
            root.SetFullRect();
            UIManager.BindUI("Start", root);
        });
    }
    public void OnUpdate()
    {

    }
}
