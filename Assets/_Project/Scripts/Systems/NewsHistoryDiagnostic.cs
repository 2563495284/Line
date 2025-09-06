using UnityEngine;

/// <summary>
/// 新闻历史记录系统诊断脚本
/// 用于检查系统状态和排查问题
/// </summary>
public class NewsHistoryDiagnostic : MonoBehaviour
{
    [Header("诊断设置")]
    [SerializeField] private bool autoCheckOnStart = true;
    [SerializeField] private bool showDebugInfo = true;

    private void Start()
    {
        if (autoCheckOnStart)
        {
            Invoke("RunDiagnostic", 1f); // 延迟1秒运行，确保其他系统已初始化
        }
    }

    /// <summary>
    /// 运行诊断检查
    /// </summary>
    public void RunDiagnostic()
    {
        Debug.Log("=== 开始新闻历史记录系统诊断 ===");

        CheckNewsSystem();
        CheckNewsHistorySystem();
        CheckNewsHistoryUI();
        CheckEventSubscriptions();

        Debug.Log("=== 诊断完成 ===");
    }

    /// <summary>
    /// 检查新闻系统
    /// </summary>
    private void CheckNewsSystem()
    {
        Debug.Log("--- 检查新闻系统 ---");

        if (NewsSystem.Instance != null)
        {
            Debug.Log($"✓ NewsSystem实例存在，ID: {NewsSystem.Instance.GetInstanceID()}");
            Debug.Log($"✓ NewsSystem状态: {NewsSystem.Instance.GetStatusInfo()}");
        }
        else
        {
            Debug.LogError("✗ NewsSystem实例不存在!");
        }
    }

    /// <summary>
    /// 检查新闻历史记录系统
    /// </summary>
    private void CheckNewsHistorySystem()
    {
        Debug.Log("--- 检查新闻历史记录系统 ---");

        if (NewsHistorySystem.Instance != null)
        {
            Debug.Log($"✓ NewsHistorySystem实例存在，ID: {NewsHistorySystem.Instance.GetInstanceID()}");

            var allHistory = NewsHistorySystem.Instance.GetAllHistory();
            Debug.Log($"✓ 总历史记录数量: {allHistory.Count}");

            var visibleHistory = NewsHistorySystem.Instance.GetVisibleHistory();
            Debug.Log($"✓ 可见历史记录数量: {visibleHistory.Count}");
        }
        else
        {
            Debug.LogError("✗ NewsHistorySystem实例不存在!");
        }
    }

    /// <summary>
    /// 检查新闻历史记录UI
    /// </summary>
    private void CheckNewsHistoryUI()
    {
        Debug.Log("--- 检查新闻历史记录UI ---");

        var historyUI = FindObjectOfType<NewsHistoryUI>();
        if (historyUI != null)
        {
            Debug.Log($"✓ NewsHistoryUI实例存在，ID: {historyUI.GetInstanceID()}");
            Debug.Log($"✓ 当前展开状态: {historyUI.IsExpanded()}");

            // 检查按钮状态
            historyUI.CheckButtonStatus();
        }
        else
        {
            Debug.LogError("✗ NewsHistoryUI实例不存在!");
        }
    }

    /// <summary>
    /// 检查事件订阅
    /// </summary>
    private void CheckEventSubscriptions()
    {
        Debug.Log("--- 检查事件订阅 ---");

        if (NewsHistorySystem.Instance != null)
        {
            // 尝试手动触发事件来测试订阅
            var visibleHistory = NewsHistorySystem.Instance.GetVisibleHistory();
            Debug.Log($"✓ 当前可见历史记录: {visibleHistory.Count} 条");

            if (visibleHistory.Count > 0)
            {
                Debug.Log($"✓ 最新新闻: {visibleHistory[visibleHistory.Count - 1].title}");
            }
        }
    }

    /// <summary>
    /// 测试新闻播报
    /// </summary>
    public void TestNewsBroadcast()
    {
        Debug.Log("=== 测试新闻播报 ===");

        if (NewsSystem.Instance != null)
        {
            string testTitle = $"测试新闻 {System.DateTime.Now:HH:mm:ss}";
            string testContent = "这是一个测试新闻，用于验证历史记录系统是否正常工作。";

            NewsSystem.Instance.BroadcastNews(testTitle, testContent, NewsType.Info, 5f);
            Debug.Log($"已播报测试新闻: {testTitle}");

            // 延迟检查结果
            Invoke("CheckTestResult", 1f);
        }
        else
        {
            Debug.LogError("NewsSystem实例不存在，无法测试新闻播报!");
        }
    }

    /// <summary>
    /// 检查测试结果
    /// </summary>
    private void CheckTestResult()
    {
        Debug.Log("=== 检查测试结果 ===");

        if (NewsHistorySystem.Instance != null)
        {
            var visibleHistory = NewsHistorySystem.Instance.GetVisibleHistory();
            Debug.Log($"测试后可见历史记录数量: {visibleHistory.Count}");

            if (visibleHistory.Count > 0)
            {
                var latestNews = visibleHistory[visibleHistory.Count - 1];
                Debug.Log($"最新新闻: {latestNews.title} - {latestNews.content}");
            }
        }
    }

    /// <summary>
    /// 清除所有历史记录
    /// </summary>
    public void ClearAllHistory()
    {
        Debug.Log("=== 清除所有历史记录 ===");

        if (NewsHistorySystem.Instance != null)
        {
            NewsHistorySystem.Instance.ClearAllHistory();
            Debug.Log("已清除所有历史记录");
        }
    }

    /// <summary>
    /// 键盘快捷键
    /// </summary>
    private void Update()
    {
        // D键运行诊断
        if (Input.GetKeyDown(KeyCode.D))
        {
            RunDiagnostic();
        }

        // T键测试新闻播报
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestNewsBroadcast();
        }

        // C键清除历史
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearAllHistory();
        }
    }

    /// <summary>
    /// 显示调试信息
    /// </summary>
    private void OnGUI()
    {
        if (!showDebugInfo) return;

        GUI.Label(new Rect(10, 10, 400, 120),
            "新闻历史记录系统诊断\n" +
            "D键: 运行诊断\n" +
            "T键: 测试新闻播报\n" +
            "C键: 清除历史记录");

        if (NewsHistorySystem.Instance != null)
        {
            var visibleHistory = NewsHistorySystem.Instance.GetVisibleHistory();
            GUI.Label(new Rect(10, 140, 400, 50),
                $"可见历史记录: {visibleHistory.Count} 条");
        }
    }
}
