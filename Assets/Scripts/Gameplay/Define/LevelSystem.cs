
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void CMDReaction<T>(T cmd) where T : LevelCommand;
public delegate void CMDReaction(LevelCommand cmd);
public delegate IEnumerator CMDPerformer<T>(T cmd) where T : LevelCommand;
public delegate IEnumerator CMDProcessor(LevelCommand cmd);
[DynamicClass(EDynamicSerial.LevelSystem)]
public class LevelSystem
{

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    protected LevelModel Model => GM.Ins.Level.model;
    protected void BindProcessor<T>(CMDPerformer<T> performer) where T : LevelCommand => GM.Ins.Level.BindProcessor(performer);
    protected void UnbindProcessor<T>() where T : LevelCommand => GM.Ins.Level.UnbindProcessor<T>();

    protected void AddRection<T>(CMDReaction<T> reaction, ReactionTiming timing) where T : LevelCommand => GM.Ins.Level.AddRection(reaction, timing);

    protected void RemoveRection<T>(CMDReaction<T> reaction, ReactionTiming timing) where T : LevelCommand => GM.Ins.Level.RemoveRection(reaction, timing);
    protected IEnumerator AwaitCMD(LevelCommand cmd) => GM.Ins.Level.AwaitCMD(cmd);
    protected Coroutine ExeCMD(LevelCommand cmd) => GM.Ins.Level.ExeCMD(cmd);
    public IEnumerator Perform(string key, object args = null) => GM.Ins.Level.Perform(key, args);
    protected void Notify(string key, object args = null) => GM.Ins.Level.Notify(key, args);

}