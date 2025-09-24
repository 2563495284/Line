using System;
using System.Collections;
using System.Collections.Generic;
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
    public abstract IEnumerator Run(IEffectEmitter from);
    public virtual bool CanBeEffect(LevelModel model) { return true; }
}
[Serializable]
public abstract class EffectWithTarget : EffectBase
{
    public abstract IEnumerator Run(IEffectEmitter from, IEffectReceiver to);
    public virtual bool CanBeEffect(LevelModel model, IEffectReceiver receiver) { return true; }

}