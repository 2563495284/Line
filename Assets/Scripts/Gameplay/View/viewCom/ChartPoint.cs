using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
public class ChartCoord
{
    public float price = 0;
    public int index = 0;
    public ChartCoord(int x, float y)
    {
        index = x;
        price = y;
    }
}
public class ChartPoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshPro stateLbl;
    [SerializeField] private TextMeshPro priceLbl;
    public ChartCoord crd;
    public void SetState(PointState state)
    {
        switch (state)
        {
            case PointState.Buy:
                stateLbl.text = "买";
                break;
            case PointState.Sell:
                stateLbl.text = "卖";
                break;
            case PointState.Bullish:
            case PointState.PredictionBullish:
                stateLbl.text = "多";
                break;
            case PointState.Bearish:
            case PointState.PredictionBearish:
                stateLbl.text = "空";
                break;
        }
    }
    public void Init(ChartCoord crd)
    {
        this.crd = crd;
        stateLbl.text = "";
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        priceLbl.text = crd.price.ToString("F2");
        priceLbl.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        priceLbl.gameObject.SetActive(false);
    }
}