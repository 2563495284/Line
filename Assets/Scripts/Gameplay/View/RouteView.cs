using TMPro;
using UnityEngine;

public class RouteView : LevelView
{
    public Transform lastRoot;
    public Transform currentRoot;
    public Transform nextRoot;
    public SpriteButton button;
    protected override void OnAwake()
    {
        base.OnAwake();
        button.onClick.AddListener(OnEnterRouteNode);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Register(EventConst.EnterRouteView, ShowView);
        Register(EventConst.EnterLevelView, ShowView);
        Register(EventConst.EnterShopView, HideView);
        Register(EventConst.EnterRoundView, HideView);
    }
    public override void OnExit()
    {
        base.OnExit();
        Unregister(EventConst.EnterRouteView, ShowView);
        Unregister(EventConst.EnterLevelView, ShowView);
        Unregister(EventConst.EnterShopView, HideView);
        Unregister(EventConst.EnterRoundView, HideView);
    }
    private void OnEnterRouteNode()
    {
        //TODO 进入操盘对局
        ExeCMD(new FinishRouteCMD());
    }
    private void ShowView()
    {
        gameObject.SetActive(true);
        UpdateRoute();
    }
    private void HideView()
    {
        gameObject.SetActive(false);
    }
    private void UpdateRoute()
    {
        LevelModel model = Model;
        LevelConfig cfg = Cfg;
        int idx = model.curRouteIdx;
        if (idx == 1)
            lastRoot.gameObject.SetActive(false);
        else
        {
            lastRoot.gameObject.SetActive(true);
            SetRouteData(lastRoot, idx - 1);
        }
        SetRouteData(currentRoot, idx);
        if (idx < cfg.routeList.Count)
        {
            nextRoot.gameObject.SetActive(true);
            SetRouteData(nextRoot, idx + 1);
        }
        else
            nextRoot.gameObject.SetActive(false);

    }
    private void SetRouteData(Transform root, int idx)
    {
        LevelConfig cfg = Cfg;
        root.Find("title").GetComponent<TextMeshPro>().text = $"交易日{idx}";
        root.Find("target").GetComponent<TextMeshPro>().text = $"达到{MUtils.FormatNumber(cfg.routeList[idx - 1])}总资产";

    }
}