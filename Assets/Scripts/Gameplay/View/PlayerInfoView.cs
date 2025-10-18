using System.Collections.Generic;
using TMPro;

public class PlayerInfoView : LevelView
{
    public TextMeshPro totalMoney;
    public TextMeshPro cash;
    public PlayerAttrList attrList;
    public List<DiceProp> dices = new();
    public override void OnEnter()
    {
        base.OnEnter();
        RefreshPlayerInfo();
        Register(EventConst.UpdateMoneyUI, RefreshMoney);
        Register(EventConst.UpdatePlayerAttr, RefreshAttr);
    }
    public override void OnExit()
    {
        base.OnExit();
        Unregister(EventConst.UpdateMoneyUI, RefreshMoney);
        Unregister(EventConst.UpdatePlayerAttr, RefreshAttr);
    }
    private void RefreshMoney()
    {
        totalMoney.text = $"总资产:{Model.money}";
        cash.text = $"现金：{Model.cash}";
    }
    private void RefreshAttr()
    {
        attrList.SetData(Model.attrInfo);
    }
    private void RefreshPlayerInfo()
    {
        RefreshMoney();
        RefreshAttr();
    }
}