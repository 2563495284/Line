using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    protected LevelController Level => GM.Ins.Level;
    protected LevelModel Model => Level.model;
    protected LevelConfig Cfg => Level.cfg;
    protected ViewController Ctrl => Level.View;
    private Dictionary<string, ECListener> wrappedMethods = new();
    private Dictionary<string, Func<object, IEnumerator>> wrappedPerforms = new();
    private Dictionary<ReactionTiming, Dictionary<Type, Delegate>> wrappedReaction = new()
    {
        {ReactionTiming.POST,new()},
        {ReactionTiming.PRE,new()},
    };
    private void Awake()
    {
        Ctrl.AddView(this);
        OnAwake();
    }
    protected virtual void OnAwake()
    {

    }
    public virtual void OnEnter()
    {

    }
    public virtual void OnExit()
    {

    }
    protected void ExeCMD(LevelCommand cmd)
    {
        Level.ExeCMD(cmd);
    }
    protected void Register(string key, Action act)
    {
        Level.View.On(key, act);
    }
    protected void Register<T>(string key, Action<T> evt)
    {
        Level.View.On(key, Wrapper(key, evt));
    }
    protected void Unregister(string key, Action act)
    {
        Level.View.Off(key, act);
    }
    protected void Unregister<T>(string key, Action<T> evt)
    {
        Level.View.Off(key, Wrapper(key, evt));
    }

    protected void Register<T>(CMDReaction<T> reaction, ReactionTiming timing) where T : LevelCommand
    {
        wrappedReaction[timing].Add(typeof(T), reaction);
        Level.AddRection(reaction, timing);
    }
    protected void Register<T>(Action reaction, ReactionTiming timing) where T : LevelCommand
    {
        Level.AddRection(Wrapper<T>(reaction, timing), timing);
    }

    protected void Unregister<T>() where T : LevelCommand
    {
        if (wrappedReaction[ReactionTiming.PRE].ContainsKey(typeof(T)))
            Level.RemoveRection(wrappedReaction[ReactionTiming.PRE][typeof(T)] as CMDReaction<T>, ReactionTiming.POST);
        if (wrappedReaction[ReactionTiming.POST].ContainsKey(typeof(T)))
            Level.RemoveRection(wrappedReaction[ReactionTiming.POST][typeof(T)] as CMDReaction<T>, ReactionTiming.POST);

    }

    protected void Bind(string key, Func<IEnumerator> perform)
    {
        Level.View.BindPerform(key, PerformWrapper(key, perform));
    }
    protected void Bind<T>(string key, Func<T, IEnumerator> perform)
    {
        Level.View.BindPerform(key, PerformWrapper(key, perform));
    }
    protected void Unbind(string key, Func<IEnumerator> perform)
    {
        Level.View.UnbindPerform(key, PerformWrapper(key, perform));
    }
    protected void Unbind<T>(string key, Func<T, IEnumerator> perform)
    {
        Level.View.UnbindPerform(key, PerformWrapper(key, perform));
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
    private CMDReaction<T> Wrapper<T>(Action action, ReactionTiming timing) where T : LevelCommand
    {
        Type type = typeof(T);
        if (wrappedReaction[timing].ContainsKey(type))
            return wrappedReaction[timing][type] as CMDReaction<T>;
        CMDReaction<T> reaction = (T cmd) => action.Invoke();
        wrappedReaction[timing].Add(type, reaction);
        return reaction;
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