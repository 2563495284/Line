using System;
using UnityEngine.EventSystems;
//coord横坐标为int，从1开始，纵坐标为float，中点为历史均价
public class StockChart : LevelView, IPointerEnterHandler, IPointerExitHandler
{
    public int stockId;


    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void PreviewEffect()
    {
        throw new NotImplementedException();
    }
}