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
        if (BuffSystem.Ins == null)
        {
            Debug.LogError("BuffSystemUIIntegration: BuffSystem实例未找到！");
            return;
        }

        if (BuffUIManager.Ins == null)
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
        BuffUIManager.Ins.RefreshDisplay();
    }



    // /// <summary>
    // /// 播放Buff过期动画
    // /// </summary>
    // public void PlayBuffExpiredAnimation(string buffName)
    // {
    //     if (enableUIIntegration && BuffUIManager.Ins != null)
    //     {
    //         BuffUIManager.Ins.PlayBuffExpiredAnimation(buffName);
    //     }
    // }

}
