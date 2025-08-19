using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private MoneyDisplay moneyDisplay;

    [SerializeField] private List<StockDisplay> stockDisplays;

    [SerializeField] private ValuesDisplay valuesDisplay;


    public void UpdateMoneyText(float currentMoney)
    {
        moneyDisplay.UpdateMoney(currentMoney);
    }

    public void UpdateStockText(EStockType stockType, int currentStock)
    {
        foreach (var stockDisplay in stockDisplays)
        {
            if (stockDisplay.stockType == stockType)
            {
                stockDisplay.UpdateStock(currentStock);
            }
        }
    }

    public void UpdateAllValuesText()
    {
        valuesDisplay.UpdateValues();
    }
}