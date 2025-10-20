using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum ERoundResult
{
    Waiting,
    Backrupt,
    Finish,
}
public class LevelController : ControllBase
{

    private CommandCtrlProxy cmdCtrl;
    private List<LevelSystem> systems = new();
    public LevelModel model;
    public LevelConfig cfg;
    public LevelController(LevelRoot root, LevelConfig cfg) : base(root)
    {
        cmdCtrl = new CommandCtrlProxy(this);
        View = new ViewController(root);
        this.cfg = cfg;
        model = new LevelModel(cfg);
    }

    public ViewController View { get; private set; }
    public bool IsOver { get; set; }
    public void Win()
    {
        Notify(EventConst.GameWin);
    }
    public void Lose()
    {
        Notify(EventConst.GameLose);
    }
    public override void OnEnter()
    {
        //Add Class
        base.OnEnter();
        View.OnEnter();
        List<Type> systemCls = LoadManager.Ins.GetAllSerialClass(EDynamicSerial.LevelSystem);
        systems.Clear();
        systemCls.ForEach(c => systems.Add(Activator.CreateInstance(c) as LevelSystem));
        systems.ForEach(e => e.OnEnter());

        cmdCtrl.SubscribeReaction<FinishRoundCMD>(FinishRoundReaction, ReactionTiming.POST);
        cmdCtrl.SubscribeReaction<BankruptCMD>(BackruptReaction, ReactionTiming.POST);

        //Launch
        InvokeAsync(StartLevelFlow(), "LevelFlow");
    }
    public override void OnExit()
    {
        cmdCtrl.UnsubscribeReaction<FinishRoundCMD>(FinishRoundReaction, ReactionTiming.POST);
        cmdCtrl.UnsubscribeReaction<BankruptCMD>(BackruptReaction, ReactionTiming.POST);

        //Exit
        base.OnExit();
        View.OnExit();
        View = null;

        //Destory
        systems.ForEach(e => e.OnExit());
        systems.Clear();
        GameObject.Destroy(Root.gameObject);
    }
    [TagEnumerator("LevelFlow")]
    private IEnumerator StartLevelFlow()
    {
        yield return AwaitCMD(new StartLevelCMD());
        while (true)
        {
            yield return AwaitCMD(new RouteRunCMD());
            roundResult = 0;
            yield return AwaitCMD(new RoundRunCMD());
            if (roundResult == ERoundResult.Backrupt)
            {
                Notify(EventConst.GameLose);
                break;
            }
            else if (cfg.routeList[model.curRouteIdx - 1] > model.money)
            {
                Notify(EventConst.GameLose);
                break;
            }
            else if (model.curRouteIdx == cfg.routeList.Count)
            {
                Notify(EventConst.GameWin);
                break;
            }
            model.curRouteIdx++;
            yield return AwaitCMD(new ShopRunCMD());
        }
        yield return AwaitCMD(new EndLevelCMD());
    }
    public IEnumerator Perform(string key, object args = null)
    {
        GM.AddLog("Perform: " + key, LogMsgType.Info);
        View.Send(key);
        return View.RequestPerform(key, args);
    }

    #region 局内指令处理 ==========================
    public bool IsInPerform => cmdCtrl.QueueCoroutine != null;
    public void BindProcessor<T>(CMDPerformer<T> performer) where T : LevelCommand
    {
        cmdCtrl.AttachProcessor(performer);
    }
    public void UnbindProcessor<T>() where T : LevelCommand
    {
        cmdCtrl.DetachProcessor<T>();
    }

    public void AddRection<T>(CMDReaction<T> reaction, ReactionTiming timing) where T : LevelCommand
    {
        cmdCtrl.SubscribeReaction(reaction, timing);
    }

    public void RemoveRection<T>(CMDReaction<T> reaction, ReactionTiming timing) where T : LevelCommand
    {
        cmdCtrl.UnsubscribeReaction(reaction, timing);
    }
    public IEnumerator AwaitCMD(LevelCommand cmd)
    {
        return cmdCtrl.ExeCMD(cmd);
    }
    public Coroutine ExeCMD(LevelCommand cmd)
    {
        if (GM.Ins.IsTrackCoroutine)
            return Root.StartTrackedCoroutine(AwaitCMD(cmd), "Execute");
        else
            return Root.StartCoroutine(AwaitCMD(cmd));
    }
    #endregion
    public void Notify(string key, object args = null)
    {
        GM.AddLog("Notify: " + key, LogMsgType.Info);
        View.Send(key, args);
    }



    private ERoundResult roundResult = ERoundResult.Waiting;
    [TagEnumerator("Backrupt")]
    private void BackruptReaction(BankruptCMD cmd)
    {
        roundResult = ERoundResult.Backrupt;
    }
    [TagEnumerator("FinishRound")]
    private void FinishRoundReaction(FinishRoundCMD cmd)
    {
        roundResult = ERoundResult.Finish;
    }
}