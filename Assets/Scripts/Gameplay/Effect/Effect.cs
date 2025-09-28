using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public interface IEffectEmitter
{

}
public interface IEffectReceiver
{

}
public abstract class EffectBase
{
    protected IEnumerator ExeCMD(LevelCommand cmd)
    {
        return GM.Ins.Level.ExeCMD(cmd);
    }
    public virtual List<LevelCommand> GetSubCmds()
    {
        return new();
    }
}
[Serializable]
public abstract class Effect : EffectBase
{
    public Effect(string[] args)
    {

    }
    public abstract IEnumerator Run(IEffectEmitter from);
    public virtual bool CanBeEffect(LevelModel model) { return true; }
}
[Serializable]
public abstract class EffectWithTarget : EffectBase
{
    public EffectWithTarget(string[] args)
    {

    }
    public abstract IEnumerator Run(IEffectEmitter from, IEffectReceiver to);
    public virtual bool CanBeEffect(LevelModel model, IEffectReceiver receiver) { return true; }

}
public static class EffectFactory
{
    public static List<T> GetEffects<T>(IReadOnlyList<string> effectStr) where T : EffectBase
    {
        List<T> res = new();
        for (int i = 0; i < effectStr.Count; i++)
        {
            IReadOnlyList<string> subStr = effectStr[i].Split('/');
            T effect = Activator.CreateInstance(Type.GetType("Effect_" + subStr[0]), subStr.Skip(1).ToArray()) as T;
            res.Add(effect);
        }
        return res;
    }
}