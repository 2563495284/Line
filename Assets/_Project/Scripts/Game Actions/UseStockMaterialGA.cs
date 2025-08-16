using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用股票作为材料的GameAction
/// </summary>
public class UseStockMaterialGA : GameAction
{
    public EStockType StockType { get; set; }
    public int Amount { get; set; }

    public UseStockMaterialGA(EStockType stockType, int amount)
    {
        StockType = stockType;
        Amount = amount;
    }
}
