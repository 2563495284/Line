using TMPro;
using UnityEngine;

public class SettlementView : LevelView
{
    public SpriteButton backBtn;
    public Transform winPanel;
    public Transform losePanel;
    public GameObject root;
    protected override void OnAwake()
    {
        base.OnAwake();
        backBtn.onClick.AddListener(OnBackMenu);
    }
    private void OnBackMenu()
    {
        GM.Ins.ExitLevel();
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Register(EventConst.GameWin, ShowWin);
        Register(EventConst.GameLose, ShowLose);
    }
    public override void OnExit()
    {
        base.OnExit();
        Unregister(EventConst.GameWin, ShowWin);
        Unregister(EventConst.GameLose, ShowLose);
    }
    private void ShowWin()
    {
        root.SetActive(true);
        losePanel.gameObject.SetActive(false);
        winPanel.gameObject.SetActive(true);
        SetText(winPanel, "total", $"总资金：{Model.money}");
        SetText(winPanel, "cash", $"总消费：{Model.consumeCash}");
        SetText(winPanel, "max", $"单日最大收益：{Model.maxBonusOneDay}");
    }
    private void ShowLose()
    {
        root.SetActive(true);
        losePanel.gameObject.SetActive(true);
        winPanel.gameObject.SetActive(false);
        SetText(losePanel, "total", $"总资金：{Model.money}");
        SetText(losePanel, "cash", $"总消费：{Model.consumeCash}");
        SetText(losePanel, "max", $"单日最大收益：{Model.maxBonusOneDay}");
        SetText(losePanel, "loseInfo", $"破产于：交易日{Model.curRouteIdx}（达到${MUtils.FormatNumber(Cfg.routeList[Model.curRouteIdx - 1])}资产");
    }
    private void SetText(Transform root, string child, string msg)
    {
        root.Find(child).GetComponent<TextMeshPro>().text = msg;
    }
    public void Hide()
    {
        root.SetActive(false);
    }

}