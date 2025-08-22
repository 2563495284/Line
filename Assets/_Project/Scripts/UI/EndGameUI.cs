using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 结束页UI：显示胜利/失败与重启人生按钮
/// </summary>
public class EndGameUI : MonoBehaviour
{
    [Header("UI 组件")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI detailText;
    [SerializeField] private Button restartButton;

    private System.Action onRestart;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        HideImmediate();
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => onRestart?.Invoke());
        }
    }

    public void Show(GameOutcome outcome, float totalAsset, float target, System.Action restartAction)
    {
        // 检查组件是否仍然有效
        if (this == null || gameObject == null || canvasGroup == null) return;

        onRestart = restartAction;
        string title = outcome == GameOutcome.Win ? "胜利" : "失败";
        string detail = outcome == GameOutcome.Win
            ? $"总资产 {Mathf.FloorToInt(totalAsset):N0} ≥ {Mathf.FloorToInt(target):N0}"
            : $"总资产 {Mathf.FloorToInt(totalAsset):N0} < {Mathf.FloorToInt(target):N0}";

        if (titleText != null) titleText.text = title;
        if (detailText != null) detailText.text = detail;

        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void HideImmediate()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}


