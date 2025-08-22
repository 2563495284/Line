using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 右下角目标进度UI：显示总资产/目标 与 倒计时天数
/// </summary>
public class GoalProgressUI : MonoBehaviour
{
    [Header("UI 组件")]
    [SerializeField] private TextMeshProUGUI daysText;     // 倒计时n天
    [SerializeField] private Slider progressBar;


    public void UpdateUI(float totalAsset, float target, float progress, int daysLeft)
    {
        if (daysText != null)
        {
            float remaining = target - totalAsset;
            daysText.text = $"背负巨债 {Mathf.FloorToInt(target):N0}， 剩余<color=red><size=150%>{Mathf.FloorToInt(remaining):N0}</size></color>，" + $"倒计时 {daysLeft} 天";
        }
        if (progressBar != null)
        {
            progressBar.normalizedValue = progress;
        }
    }
}


