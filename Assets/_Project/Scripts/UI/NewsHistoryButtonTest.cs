using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 新闻历史记录按钮测试脚本
/// 用于调试按钮点击问题
/// </summary>
public class NewsHistoryButtonTest : MonoBehaviour
{
    [Header("测试按钮")]
    [SerializeField] private Button testExpandButton;
    [SerializeField] private Button testClearButton;

    [Header("目标UI")]
    [SerializeField] private NewsHistoryUI newsHistoryUI;

    private void Start()
    {
        SetupTestButtons();

        // 如果没有指定UI，尝试自动查找
        if (newsHistoryUI == null)
        {
            newsHistoryUI = FindObjectOfType<NewsHistoryUI>();
        }
    }

    /// <summary>
    /// 设置测试按钮
    /// </summary>
    private void SetupTestButtons()
    {
        if (testExpandButton != null)
        {
            testExpandButton.onClick.AddListener(TestExpandButton);
            Debug.Log("测试展开按钮已设置");
        }

        if (testClearButton != null)
        {
            testClearButton.onClick.AddListener(TestClearButton);
            Debug.Log("测试清除按钮已设置");
        }
    }

    /// <summary>
    /// 测试展开按钮
    /// </summary>
    private void TestExpandButton()
    {
        Debug.Log("=== 测试展开按钮 ===");

        if (newsHistoryUI != null)
        {
            Debug.Log("找到NewsHistoryUI，检查按钮状态...");
            newsHistoryUI.CheckButtonStatus();

            // 尝试手动切换状态
            bool currentState = newsHistoryUI.IsExpanded();
            Debug.Log($"当前展开状态: {currentState}");

            newsHistoryUI.SetExpanded(!currentState);
            Debug.Log($"已手动切换状态为: {!currentState}");
        }
        else
        {
            Debug.LogWarning("未找到NewsHistoryUI!");
        }
    }

    /// <summary>
    /// 测试清除按钮
    /// </summary>
    private void TestClearButton()
    {
        Debug.Log("=== 测试清除按钮 ===");

        if (newsHistoryUI != null)
        {
            Debug.Log("找到NewsHistoryUI，尝试清除历史...");

            if (NewsHistorySystem.Instance != null)
            {
                NewsHistorySystem.Instance.ClearAllHistory();
                Debug.Log("已清除所有历史记录");
            }
            else
            {
                Debug.LogWarning("NewsHistorySystem实例不存在!");
            }
        }
        else
        {
            Debug.LogWarning("未找到NewsHistoryUI!");
        }
    }

    /// <summary>
    /// 键盘快捷键测试
    /// </summary>
    private void Update()
    {
        // T键测试展开按钮
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestExpandButton();
        }

        // Y键测试清除按钮
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TestClearButton();
        }

        // U键检查UI状态
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (newsHistoryUI != null)
            {
                newsHistoryUI.CheckButtonStatus();
            }
        }
    }

    /// <summary>
    /// 显示调试信息
    /// </summary>
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 100),
            "新闻历史记录按钮测试\n" +
            "T键: 测试展开按钮\n" +
            "Y键: 测试清除按钮\n" +
            "U键: 检查UI状态");

        if (newsHistoryUI != null)
        {
            GUI.Label(new Rect(10, 120, 400, 50),
                $"当前展开状态: {newsHistoryUI.IsExpanded()}");
        }
    }
}
