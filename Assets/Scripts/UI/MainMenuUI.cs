using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 主菜单 UI 控制器。
/// 挂载在主菜单场景的 Canvas 根节点下。
///
/// 按钮绑定关系（Inspector 中拖拽）：
///   newGameButton     → 新游戏
///   loadGameButton    → 继续游戏（仅有存档时显示）
///   deleteGameButton  → 删除存档（仅有存档时显示）
///
/// 弹窗（confirmDeletePanel）：二次确认删档，避免误操作。
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("主按钮")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button deleteGameButton;

    [Header("存档信息面板")]
    [SerializeField] private GameObject saveInfoPanel;
    [SerializeField] private TextMeshProUGUI saveTimeText;
    [SerializeField] private TextMeshProUGUI saveMoneyText;

    [Header("确认删档弹窗")]
    [SerializeField] private GameObject confirmDeletePanel;
    [SerializeField] private Button confirmDeleteYesButton;
    [SerializeField] private Button confirmDeleteNoButton;

    [Header("覆盖存档弹窗（新游戏时已有存档）")]
    [SerializeField] private GameObject confirmOverwritePanel;
    [SerializeField] private Button confirmOverwriteYesButton;
    [SerializeField] private Button confirmOverwriteNoButton;

    private void Start()
    {
        BindButtons();
        RefreshUI();
    }

    private void BindButtons()
    {
        newGameButton.onClick.AddListener(OnNewGameClicked);
        loadGameButton.onClick.AddListener(OnLoadGameClicked);
        deleteGameButton.onClick.AddListener(OnDeleteGameClicked);

        if (confirmDeleteYesButton) confirmDeleteYesButton.onClick.AddListener(OnConfirmDeleteYes);
        if (confirmDeleteNoButton)  confirmDeleteNoButton.onClick.AddListener(OnConfirmDeleteNo);

        if (confirmOverwriteYesButton) confirmOverwriteYesButton.onClick.AddListener(OnConfirmOverwriteYes);
        if (confirmOverwriteNoButton)  confirmOverwriteNoButton.onClick.AddListener(OnConfirmOverwriteNo);
    }

    // ── UI 刷新 ────────────────────────────────────────────────────────────────

    private void RefreshUI()
    {
        bool hasSave = SaveManager.Instance.HasSave();

        // 继续/删除按钮只在有存档时显示
        if (loadGameButton)   loadGameButton.gameObject.SetActive(hasSave);
        if (deleteGameButton) deleteGameButton.gameObject.SetActive(hasSave);

        // 存档信息面板
        if (saveInfoPanel) saveInfoPanel.SetActive(hasSave);

        if (hasSave)
        {
            var save = SaveManager.Instance.Load();
            if (save != null)
            {
                if (saveTimeText)  saveTimeText.text  = $"上次存档：{save.saveTime}";
                if (saveMoneyText) saveMoneyText.text = $"金币：{save.Money}";
            }
        }

        // 确保弹窗默认关闭
        HideAllPanels();
    }

    private void HideAllPanels()
    {
        if (confirmDeletePanel)    confirmDeletePanel.SetActive(false);
        if (confirmOverwritePanel) confirmOverwritePanel.SetActive(false);
    }

    // ── 按钮事件 ───────────────────────────────────────────────────────────────

    private void OnNewGameClicked()
    {
        if (SaveManager.Instance.HasSave())
        {
            // 已有存档 → 弹窗询问是否覆盖
            if (confirmOverwritePanel)
            {
                confirmOverwritePanel.SetActive(true);
                return;
            }
        }
        // 无存档 → 直接开始
        GameFlowManager.Instance.StartNewGame();
    }

    private void OnLoadGameClicked()
    {
        GameFlowManager.Instance.LoadGame();
    }

    private void OnDeleteGameClicked()
    {
        if (confirmDeletePanel)
            confirmDeletePanel.SetActive(true);
    }

    // ── 确认删档弹窗 ───────────────────────────────────────────────────────────

    private void OnConfirmDeleteYes()
    {
        SaveManager.Instance.Delete();
        HideAllPanels();
        RefreshUI();
    }

    private void OnConfirmDeleteNo()
    {
        HideAllPanels();
    }

    // ── 确认覆盖存档弹窗 ───────────────────────────────────────────────────────

    private void OnConfirmOverwriteYes()
    {
        HideAllPanels();
        GameFlowManager.Instance.StartNewGame();
    }

    private void OnConfirmOverwriteNo()
    {
        HideAllPanels();
    }
}
