using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MarketNewsView : LevelView
{
    public GameObject newsPopupComPrefab;
    public GameObject newsHistoryListPrefab;
    private NewsPopupCom newsPopupCom;
    private NewsHistoryListCom newsHistoryList;

    /// <summary>
    /// 初始化UI组件
    /// </summary>
    protected override void OnInit()
    {
        newsHistoryList = Instantiate(newsHistoryListPrefab).GetComponent<NewsHistoryListCom>();
        AddCanvasCom(newsHistoryList.gameObject);
        newsHistoryList.Init();
        newsPopupCom = Instantiate(newsPopupComPrefab).GetComponent<NewsPopupCom>();
        AddCanvasCom(newsPopupCom.gameObject);
    }
    protected override void OnShow()
    {
        Register(NotifyConst.UpdateNewsHistory, OnUpdateNewsHistory);
        Register<PopupNewsArgs>(NotifyConst.PopupNews, OnPopupNews);
    }
    protected override void OnHide()
    {
        Unregister(NotifyConst.UpdateNewsHistory, OnUpdateNewsHistory);
        Unregister<PopupNewsArgs>(NotifyConst.PopupNews, OnPopupNews);
        newsPopupCom.Clear();
    }
    private void OnUpdateNewsHistory()
    {
        newsPopupCom.UpdateHistory();
    }
    /// <summary>
    /// 播报新闻
    /// </summary>
    private void OnPopupNews(PopupNewsArgs args)
    {
        newsPopupCom.PopupNews(args);
    }





}