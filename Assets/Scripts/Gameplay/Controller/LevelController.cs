using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 每局游戏生成一个LevelController
/// </summary>
public class LevelController : ControllBase
{
    public static int CardCNT = 0;
    public static int EnemyCNT = 0;
    private List<LevelSystem> systems = new();
    public LevelConfig Cfg { get; private set; }
    public int CurrectRound { get; private set; }
    private CommandCtrlProxy cmdCtrl;
    public LevelModel model { get; private set; }
    public LevelController(LevelConfig levelCfg, LevelRoot root) : base(root)
    {
        Cfg = levelCfg;
        cmdCtrl = new CommandCtrlProxy(this);
        model = new LevelModel(levelCfg);
        //【TODO】生成system
    }
    #region 生命周期 ==============================
    public IEnumerator RequestPerform(string key, object args = null)
    {
        return GM.Ins.View.RequestPerform(key, args);
    }
    public void Notify(string key, object args = null)
    {
        GM.Ins.View.Send(key, args);
    }
    //关卡开始
    public override void OnEnter()
    {
        CardCNT = 0;
        EnemyCNT = 0;

        List<Type> systemCls = LoadManager.GetAllSubCls(typeof(LevelSystem));
        systems.Clear();
        systemCls.ForEach(c => systems.Add(Activator.CreateInstance(c, this) as LevelSystem));
        cmdCtrl.AttachPerformer<NextRoundTurnCMD>(TurnRoundIE);
        systems.ForEach(e => e.EnableSystem());
        DrawCardsCMD drawCardsCMD = new(Cfg.playerData.initialDrawCount);
        AddCMD(drawCardsCMD);

    }
    //关卡结束
    public override void OnExit()
    {
        base.OnExit();
        systems.ForEach(e => e.DisableSystem());
        systems.Clear();
        GameObject.Destroy(Root.gameObject);
    }

    #endregion
    #region 局内流程处理 ===========================
    private IEnumerator TurnRoundIE(NextRoundTurnCMD cmd)
    {
        CurrectRound++;
        if (CurrectRound >= Cfg.targetRounds)
        {
            JudgeGame();
        }
        DiscardAllCardsCMD discardAllCardsCMD = new();
        yield return ExeCMD(discardAllCardsCMD);
        yield return null;
        DrawCardsCMD drawCardsCMD = new(model.CardNumPerTurn);
        yield return ExeCMD(drawCardsCMD);

        ChangeAttributeCMD changeAttributeCMD = new(EAttrType.Social, -1f);
        yield return ExeCMD(changeAttributeCMD);
        yield return ExeCMD(new RefillManaCMD());
    }
    private void JudgeGame()
    {
        //【TODO】
    }

    #endregion

    #region 局内指令处理 ==========================
    public bool IsInPerform => cmdCtrl.IsProcessingQueue;
    public void BindPerformer<T>(CMDPerformer<T> performer) where T : LevelCommand
    {
        cmdCtrl.AttachPerformer(performer);
    }
    public void UnbindPerformer<T>() where T : LevelCommand
    {
        cmdCtrl.DetachPerformer<T>();
    }

    public void AddRection<T>(Action<T> reaction, ReactionTiming timing) where T : LevelCommand
    {
        cmdCtrl.SubscribeReaction(reaction, timing);
    }

    public void RemoveRection<T>(Action<T> reaction, ReactionTiming timing) where T : LevelCommand
    {
        cmdCtrl.UnsubscribeReaction(reaction, timing);
    }
    /// <summary>
    /// 触发指令
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="OnPerformFinished"></param>
    public void AddCMD(LevelCommand cmd)
    {
        cmdCtrl.AddCMD(cmd);
    }
    public IEnumerator ExeCMD(LevelCommand cmd)
    {
        return cmdCtrl.ExeCMD(cmd);
    }
    #endregion


}