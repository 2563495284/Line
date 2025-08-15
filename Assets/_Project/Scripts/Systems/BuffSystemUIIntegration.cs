using UnityEngine;

/// <summary>
/// BuffSystem与UI系统的集成扩展
/// 用于处理BuffSystem和BuffUIManager之间的通信
/// </summary>
public class BuffSystemUIIntegration : MonoBehaviour
{
    [Header("集成设置")]
    [SerializeField] private bool enableUIIntegration = true;
    [SerializeField] private bool showDebugInfo = false;

    private void Start()
    {
        if (enableUIIntegration)
        {
            InitializeUIIntegration();
        }
    }

    /// <summary>
    /// 初始化UI集成
    /// </summary>
    private void InitializeUIIntegration()
    {
        // 等待系统初始化完成
        Invoke(nameof(SetupUICallbacks), 0.1f);
    }

    /// <summary>
    /// 设置UI回调
    /// </summary>
    private void SetupUICallbacks()
    {
        if (BuffSystem.Instance == null)
        {
            Debug.LogError("BuffSystemUIIntegration: BuffSystem实例未找到！");
            return;
        }

        if (BuffUIManager.Instance == null)
        {
            Debug.LogWarning("BuffSystemUIIntegration: BuffUIManager实例未找到，UI集成将被禁用");
            enableUIIntegration = false;
            return;
        }

        if (showDebugInfo)
        {
            Debug.Log("BuffSystemUIIntegration: UI集成已启用");
        }

        // 立即刷新一次UI
        BuffUIManager.Instance.RefreshDisplay();
    }

    /// <summary>
    /// 手动触发UI更新
    /// </summary>
    [ContextMenu("手动更新UI")]
    public void ManualUpdateUI()
    {
        if (enableUIIntegration && BuffUIManager.Instance != null)
        {
            BuffUIManager.Instance.RefreshDisplay();
            Debug.Log("已手动触发UI更新");
        }
    }

    /// <summary>
    /// 播放Buff获得动画
    /// </summary>
    public void PlayBuffGainedAnimation(string buffName)
    {
        if (enableUIIntegration && BuffUIManager.Instance != null)
        {
            BuffUIManager.Instance.PlayBuffGainedAnimation(buffName);
        }
    }

    /// <summary>
    /// 播放Buff过期动画
    /// </summary>
    public void PlayBuffExpiredAnimation(string buffName)
    {
        if (enableUIIntegration && BuffUIManager.Instance != null)
        {
            BuffUIManager.Instance.PlayBuffExpiredAnimation(buffName);
        }
    }

    /// <summary>
    /// 检查UI集成状态
    /// </summary>
    [ContextMenu("检查UI集成状态")]
    public void CheckUIIntegrationStatus()
    {
        Debug.Log("=== UI集成状态 ===");
        Debug.Log($"UI集成启用: {enableUIIntegration}");
        Debug.Log($"BuffSystem存在: {BuffSystem.Instance != null}");
        Debug.Log($"BuffUIManager存在: {BuffUIManager.Instance != null}");

        if (BuffUIManager.Instance != null)
        {
            Debug.Log($"活跃Buff数量: {BuffUIManager.Instance.GetActiveBuffCount()}");
        }
    }
}
