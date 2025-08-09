using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private MoneyDisplay moneyDisplay;

    [SerializeField] private StockDisplay stockDisplay;

    [SerializeField] private ValuesDisplay valuesDisplay;


    public void UpdateMoneyText(float currentMoney)
    {
        moneyDisplay.UpdateMoney(currentMoney);
    }

    public void UpdateStockText(int currentStock)
    {
        stockDisplay.UpdateStock(currentStock);
    }

    public void UpdateAllValuesText(float stockPrice)
    {
        valuesDisplay.UpdateValues(moneyDisplay.CurrentMoney + stockDisplay.CurrentStock * stockPrice);
    }

}