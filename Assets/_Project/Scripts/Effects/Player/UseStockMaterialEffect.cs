using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用股票材料的效果
/// </summary>
public class UseStockMaterialEffect : Effect
{
    [Header("材料消耗设置")]
    [SerializeField]
    private EStockType materialType;

    [SerializeField]
    private int materialAmount = 1;

    [Header("消耗检查")]
    [SerializeField]
    private bool checkMaterialBeforeUse = true;

    public override GameAction GetGameAction()
    {
        UseStockMaterialGA materialGA = new UseStockMaterialGA(materialType, materialAmount);
        return materialGA;
    }
}
