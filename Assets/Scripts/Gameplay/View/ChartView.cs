using System;
using System.Collections.Generic;
using UnityEngine;

public class ChartView : LevelView
{
    public GameObject chartPrefab;
    private List<StockChart> stockCharts = new();
    public Transform chartFolder;
    public float space = 5.6f;
    protected override void OnInit()
    {
        base.OnInit();
        float halfCnt = (Data.stocks.Count - 1) / 2.0f;
        for (int i = 0; i < Data.stocks.Count; i++)
        {
            StockChart chart = chartPrefab.OPGet(chartFolder).GetComponent<StockChart>();
            chart.Init(Data.stocks[i]);
            chart.transform.position = new Vector3((i - halfCnt) * space, 0, 0);
            stockCharts.Add(chart);
        }
    }
    private StockChart GetChart(int stockId)
    {
        return stockCharts.Find(e => e.StockId == stockId);
    }
    protected override void OnShow()
    {
        Register(NotifyConst.UpdateStockInfo, OnUpdateStockInfo);
        Register<SetPointStateArgs>(NotifyConst.SetPointState, OnSetPointState);
    }
    protected override void OnHide()
    {
        Unregister(NotifyConst.UpdateStockInfo, OnUpdateStockInfo);
        Unregister<SetPointStateArgs>(NotifyConst.SetPointState, OnSetPointState);

    }
    /// <summary>
    /// 股票文本信息变化
    /// </summary>
    private void OnUpdateStockInfo()
    {
        stockCharts.ForEach(e => e.UpdateChart());
    }
    private void OnSetPointState(SetPointStateArgs args)
    {
        GetChart(args.stockId).SetLastPointState(args.state);
    }
}