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

    public void UpdateValuesText()
    {
        const float stockValue = 100000;
        valuesDisplay.UpdateValues(moneyDisplay.CurrentMoney + stockDisplay.CurrentStock * stockValue);
    }

}