using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CardGoodsInfo
{
    public int cardId;
    public int price;
}
public class CardGoods : GoodsItem
{
    private CardGoodsInfo info;
    public void SetData(CardGoodsInfo info)
    {
        this.info = info;
    }
}
