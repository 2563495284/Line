using System.Collections;

public class RoundSystem : LevelSystem
{
    public override void OnEnter()
    {
        base.OnEnter();
        BindProcessor<StartLevelCMD>(StartLevelProcessor);
        BindProcessor<EndLevelCMD>(EndLevelProcessor);
        BindProcessor<RoundRunCMD>(RoundProcessor);
        BindProcessor<ActionPhaseCMD>(ActionPhaseProcessor);
        BindProcessor<TradePhaseCMD>(TradePhaseProcessor);
        BindProcessor<SummaryPhaseCMD>(SummaryPhaseProcessor);
        BindProcessor<BankruptCMD>(BankruptProcessor);
        BindProcessor<FinishActionCMD>(FinishActionProcessor);
        BindProcessor<FinishTradeCMD>(FinishTradeProcessor);
        BindProcessor<CheckRoundCMD>(CheckRoundProcessor);
        BindProcessor<RoundStartCMD>(RoundStartProcessor);
        BindProcessor<RoundEndCMD>(RoundEndProcessor);
    }
    public override void OnExit()
    {
        base.OnExit();
        UnbindProcessor<StartLevelCMD>();
        UnbindProcessor<EndLevelCMD>();
        UnbindProcessor<RoundRunCMD>();
        UnbindProcessor<ActionPhaseCMD>();
        UnbindProcessor<TradePhaseCMD>();
        UnbindProcessor<SummaryPhaseCMD>();
        UnbindProcessor<BankruptCMD>();
        UnbindProcessor<FinishActionCMD>();
        UnbindProcessor<FinishTradeCMD>();
        UnbindProcessor<CheckRoundCMD>();
        UnbindProcessor<RoundStartCMD>();
        UnbindProcessor<RoundEndCMD>();

    }
    #region  LevelFlow
    [TagEnumerator("StartLevel")]
    private IEnumerator StartLevelProcessor(StartLevelCMD cmd)
    {
        yield return Perform(EventConst.EnterLevelView);
    }
    [TagEnumerator("EndLevel")]
    private IEnumerator EndLevelProcessor(EndLevelCMD cmd)
    {
        yield return null;
    }
    [TagEnumerator("EnterRound")]
    private IEnumerator RoundProcessor(RoundRunCMD cmd)
    {
        yield return Perform(EventConst.EnterRoundView);
        yield return AwaitCMD(new RoundStartCMD()); //回合开始效果发动
        yield return AwaitCMD(new ActionPhaseCMD());//行动阶段
        yield return AwaitCMD(new TradePhaseCMD());//交易阶段
        yield return AwaitCMD(new RoundEndCMD());   //回合结束效果发动
        yield return AwaitCMD(new SummaryPhaseCMD());//回合清算
    }
    #endregion



    #region Round flow
    [TagEnumerator("ActionPhase")]
    private IEnumerator ActionPhaseProcessor(ActionPhaseCMD cmd)
    {
        yield return AwaitCMD(new DrawActionCardsCMD(5));
        finishAction = false;
        while (!finishAction)
            yield return null;

    }
    [TagEnumerator("TradePhase")]
    private IEnumerator TradePhaseProcessor(TradePhaseCMD cmd)
    {
        finishTrade = false;
        while (!finishTrade)
            yield return null;

    }
    [TagEnumerator("SummaryPhase")]
    private IEnumerator SummaryPhaseProcessor(SummaryPhaseCMD cmd)
    {
        yield return 0;
    }

    #endregion
    bool checkDirty = true;
    bool finishAction = false;
    bool finishTrade = false;
    private IEnumerator CheckRoundProcessor(CheckRoundCMD cmd)
    {
        checkDirty = true;
        yield return 0;
    }
    private IEnumerator FinishActionProcessor(FinishActionCMD cmd)
    {
        finishAction = true;
        yield return 0;
    }
    private IEnumerator BankruptProcessor(BankruptCMD cmd)
    {
        finishAction = true;
        yield return 0;
    }
    private IEnumerator FinishTradeProcessor(FinishTradeCMD cmd)
    {
        finishTrade = true;
        yield return 0;
    }
    private IEnumerator RoundStartProcessor(RoundStartCMD cmd)
    {
        yield return AwaitCMD(new UpdateStockStrategyCMD());
        yield return AwaitCMD(new ApplyStockAttrCMD(EStockAttrApplyTiming.RoundStart));
        yield return AwaitCMD(new UpdateIndustryPowerCMD());
    }
    private IEnumerator RoundEndProcessor(RoundEndCMD cmd)
    {
        yield return AwaitCMD(new CalcPerformanceCMD());
        yield return AwaitCMD(new ApplyStockAttrCMD(EStockAttrApplyTiming.BeforeSettlement));
        yield return AwaitCMD(new SettlementStockCMD());
        yield return AwaitCMD(new ApplyStockAttrCMD(EStockAttrApplyTiming.AfterSettlement));
        Model.stocks.List.ForEach(e => e.ExitRound());
        Notify(EventConst.UpdateStockView);
    }

}