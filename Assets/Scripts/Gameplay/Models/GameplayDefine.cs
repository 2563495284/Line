
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void CMDReaction<T>(T cmd) where T : LevelCommand;
public delegate void CMDReaction(LevelCommand cmd);
public delegate IEnumerator CMDPerformer<T>(T cmd) where T : LevelCommand;
public delegate IEnumerator CMDPerformer(LevelCommand cmd);
public class LevelSystem
{
    public LevelConfig Cfg => Ctrl.Cfg;
    public LevelModel Data => Ctrl.model;
    public LevelController Ctrl { get; private set; }
    public LevelSystem(LevelController ctrl)
    {
        this.Ctrl = ctrl;
    }

    public virtual void DisableSystem() { }
    public virtual void EnableSystem() { }
}
public class LevelCommand
{
    //只允许CommandCtrlProxy修改！
    public int executeIndex = 0;
    private List<LevelCommand> subCmds = new();
    /// <summary>
    /// 运行时生成的LevelCommand
    /// </summary>
    /// <param name="cmd"></param>
    public void AddSubCmd(LevelCommand cmd)
    {
        subCmds.Add(cmd);

    }
    /// <summary>
    /// 取出所有运行时生成的LevelCommand
    /// </summary>
    public List<LevelCommand> PickGen()
    {
        List<LevelCommand> res = subCmds;
        subCmds.Clear();
        return res;
    }
}