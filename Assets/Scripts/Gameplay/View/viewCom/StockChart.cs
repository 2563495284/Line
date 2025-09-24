using UnityEngine;
using UnityEngine.EventSystems;

public class StockChart : MonoBehaviour, ICardEffectTarget, IPointerEnterHandler, IPointerExitHandler
{
    public EStockType stockType { get; private set; }
    public StockModel StockData => GM.LevelData.GetStockModel(stockType);
    public void PreviewEffect()
    {
    }
    public void CancelPreviewEffect()
    {

    }

    public IEffectReceiver GetReceiver()
    {
        return StockData;
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