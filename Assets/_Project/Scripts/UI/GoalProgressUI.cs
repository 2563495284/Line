using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;

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
            float remaining = math.max(0, target - totalAsset);
            daysText.text = $"负债：{Mathf.FloorToInt(target):N0}，资产<color=green><size=150%>{Mathf.FloorToInt(totalAsset):N0}</size></color>，剩余负债<color=red><size=150%>{Mathf.FloorToInt(remaining):N0}</size></color>" + $"还债期限 {daysLeft} 天";
        }
        if (progressBar != null)
        {
            progressBar.normalizedValue = progress;
        }
    }
}


