using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
//coord横坐标为int，从1开始，纵坐标为float，中点为历史均价
public class StockChart : MonoBehaviour, ICardEffectTarget, IPointerEnterHandler, IPointerExitHandler
{
    public int StockId => Data.stockId;
    public StockModel Data { get; private set; }
    [SerializeField] private Transform rectRange;
    public Rect ChartRect => new(rectRange.position, rectRange.localScale);
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform chartFolder;
    [SerializeField] private TextMeshPro titleText;
    [SerializeField] private TextMeshPro currentPriceText;
    [SerializeField] private TextMeshPro changePercentText;
    private Vector2 scaleFactor = new Vector2(1, 1);
    private Vector2 offsetFactor = new Vector2(0, 0);
    private List<ChartPoint> chartPoints = new();

    public void PreviewEffect()
    {
    }
    public void CancelPreviewEffect()
    {

    }


    public void Init(StockModel stockData)
    {
        Data = stockData;
        titleText.text = stockData.Cfg.StockName;
        currentPriceText.text = stockData.price.ToString("F2");
    }

    public void Clear()
    {
        pointPrefab.OPClear();
    }
    public void UpdateChart()
    {
        CalcRectScale();
        chartPoints.ForEach(e => e.gameObject.OPPush());
        chartPoints.Clear();
        int startIdx = (int)offsetFactor.x;
        Vector3[] pos = new Vector3[Data.priceHistory.Count];
        for (int i = 0; i < Data.priceHistory.Count; i++)
        {
            float price = Data.priceHistory[i];
            ChartCoord crd = new ChartCoord(startIdx + i, startIdx + i);
            chartPoints.Add(GenPoint(crd));
            pos[i] = Crd2Pos(crd);
        }
        currentPriceText.text = Data.price.ToString("F2");
        lineRenderer.SetPositions(pos);
    }
    public void SetLastPointState(PointState state)
    {
        chartPoints[^1].SetState(state);
    }
    private ChartPoint GenPoint(ChartCoord crd)
    {
        ChartPoint point = pointPrefab.OPGet(chartFolder).GetComponent<ChartPoint>();
        point.Init(crd);
        point.transform.position = Crd2Pos(crd);
        return point;
    }
    private void CalcRectScale()
    {
        float maxPrice = Data.priceHistory.Max();
        float minPrice = Data.priceHistory.Min();
        const float minDelta = 5;
        if (maxPrice - minPrice < minDelta)
        {
            scaleFactor.y = ChartRect.height / minDelta;
            offsetFactor.y = (minPrice + maxPrice) / 2 - minDelta / 2;
        }
        else
        {
            scaleFactor.y = ChartRect.height / (maxPrice - minPrice);
            offsetFactor.y = minPrice;
        }
        scaleFactor.x = ChartRect.width / Data.priceHistory.Count;
        offsetFactor.x = GM.LevelData.turn - Data.priceHistory.Count + 1;
    }
    private Vector2 Crd2Pos(ChartCoord crd)
    {
        float x = (crd.index - offsetFactor.x) * scaleFactor.x + ChartRect.xMin;
        float y = (crd.price - offsetFactor.y) * scaleFactor.y + ChartRect.yMin;
        return new Vector2(x, y);
    }

    public IEffectReceiver GetReceiver()
    {
        return Data;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GM.Ins.Level.ExeCMD(new SelectCardTargetCMD
        {
            target = this
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GM.Ins.Level.ExeCMD(new CancelSelectTargetCMD
        {
            target = this
        });
    }

}