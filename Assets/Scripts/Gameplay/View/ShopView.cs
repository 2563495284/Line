using System.Collections.Generic;
using UnityEngine;

public class ShopView : LevelView
{
    public List<DiceGoods> dices = new();
    public List<CardGoods> cards = new();
    public SpriteButton continueBtn;
    protected override void OnAwake()
    {
        base.OnAwake();
        continueBtn.onClick.AddListener(OnClickContinue);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Register(EventConst.EnterLevelView, HideShop);
        Register(EventConst.EnterRouteView, HideShop);
        Register(EventConst.EnterShopView, ShowShop);
        Register(EventConst.EnterRoundView, HideShop);

    }
    private void OnClickContinue()
    {
        ExeCMD(new FinishShoppingCMD());
    }
    public override void OnExit()
    {
        base.OnExit();
        Unregister(EventConst.EnterLevelView, HideShop);
        Unregister(EventConst.EnterRouteView, HideShop);
        Unregister(EventConst.EnterShopView, ShowShop);
        Unregister(EventConst.EnterRoundView, HideShop);
    }
    private void ShowShop()
    {
        gameObject.SetActive(true);
        ShopInfo info = Model.GetShopInfo();
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetData(info.cards[i]);
        }
        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].SetData(info.dices[i]);
        }
    }
    private void HideShop()
    {
        gameObject.SetActive(false);
    }
}