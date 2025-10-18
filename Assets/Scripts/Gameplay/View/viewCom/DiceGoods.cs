using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DiceGoodsInfo
{
    public int price;
    public int diceId;
}
public class DiceGoods : GoodsItem
{
    DiceGoodsInfo goodsInfo;
    public void SetData(DiceGoodsInfo info)
    {
        goodsInfo = info;
    }
}
