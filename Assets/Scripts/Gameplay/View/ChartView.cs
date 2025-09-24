using System;
using System.Collections.Generic;

public class ChartView : LevelView
{
    public List<StockChartCom> stockCharts = new();
    protected override void OnShow()
    {
        Register(NotifyConst.UpdateStockInfo, OnUpdateStockInfo);
        Register<PredictStockArgs>(NotifyConst.PredictStock, OnPredictStock);
    }
    protected override void OnHide()
    {
        Unregister(NotifyConst.UpdateStockInfo, OnUpdateStockInfo);
        Unregister<PredictStockArgs>(NotifyConst.PredictStock, OnPredictStock);

    }
    /// <summary>
    /// 股票文本信息变化
    /// </summary>
    private void OnUpdateStockInfo()
    {
        stockCharts.ForEach(e => e.UpdateStock(Data.GetStockModel(e.stockType).holding));
    }
    private void OnPredictStock(PredictStockArgs args)
    {
        // switch (predictionCMD.PredictionType)
        // {
        //     case EPredictionType.Rise:
        //         lineView.SetPointState(PointState.PredictionBullish);
        //         break;
        //     case EPredictionType.Fall:
        //         lineView.SetPointState(PointState.PredictionBearish);
        //         break;
        // }
    }
}