using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySystem : Singleton<MoneySystem>
{
    [SerializeField] private MoneyUI moneyUI;

    [SerializeField] float initialMoney = 100000;

    private float currentMoney = 0;

    private int stock = 0;

    protected override void Awake()
    {
        base.Awake();
        currentMoney = initialMoney;
        moneyUI.UpdateMoneyText(currentMoney);
        moneyUI.UpdateStockText(stock);
        moneyUI.UpdateValuesText();
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<SpendMoneyGA>(SpendMoneyPerformer);
        ActionSystem.AttachPerformer<AddMoneyGA>(AddMoneyPerformer);
        ActionSystem.AttachPerformer<SpendStockGA>(SpendStockPerformer);
        ActionSystem.AttachPerformer<AddStockGA>(AddStockPerformer);

    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<SpendMoneyGA>();
        ActionSystem.DetachPerformer<AddMoneyGA>();
        ActionSystem.DetachPerformer<SpendStockGA>();
        ActionSystem.DetachPerformer<AddStockGA>();
    }

    private IEnumerator SpendMoneyPerformer(SpendMoneyGA action)
    {
        currentMoney -= action.Amount;
        moneyUI.UpdateMoneyText(currentMoney);
        yield return null;
    }

    private IEnumerator AddMoneyPerformer(AddMoneyGA action)
    {
        currentMoney += action.Amount;
        moneyUI.UpdateMoneyText(currentMoney);
        yield return null;
    }

    private IEnumerator SpendStockPerformer(SpendStockGA action)
    {
        stock -= action.Amount;
        moneyUI.UpdateStockText(stock);
        yield return null;
    }

    private IEnumerator AddStockPerformer(AddStockGA action)
    {
        stock += action.Amount;
        moneyUI.UpdateStockText(stock);
        yield return null;
    }
}