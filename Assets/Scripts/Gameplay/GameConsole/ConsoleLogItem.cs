using UnityEngine;
using TMPro;
using UnityEngine.Events;

// 日志数据结构
public class LogData
{
    public string message;       // 日志内容
    public LogMsgType logType;      // 日志类型
    public long timestamp;      // 时间戳（可选）
}

// 日志文本项组件（TMP实现）
public class ConsoleLogItem : MonoBehaviour
{
    [Header("文本组件")]
    [SerializeField] private TextMeshProUGUI logText;

    [Header("文本样式设置")]
    [SerializeField] private float fontSize = 14f;
    [SerializeField] private float lineSpacing = 1.1f;  // 行间距倍数
    [SerializeField] private float padding = 8f;        // 内边距（上下各一半）

    [Header("日志类型颜色")]
    [SerializeField] private Color infoColor = Color.white;
    [SerializeField] private Color warnColor = new Color(1f, 0.6f, 0.27f);
    [SerializeField] private Color errorColor = new Color(1f, 0.27f, 0.27f);
    [SerializeField] private Color echoColor = new Color(0.28f, 1f, 1f);
    [SerializeField] private Color returnColor = new Color(0.254f, 0.77f, 1f);
    public UnityEvent<float> OnHeightChanged = new UnityEvent<float>();
    protected RectTransform RT => transform as RectTransform;
    public float Height => RT.sizeDelta.y;
    private float baseHeight; // 基础高度缓存

    protected void Awake()
    {
        if (logText != null)
        {
            // 初始化TMP设置
            logText.fontSize = fontSize;
            logText.lineSpacing = lineSpacing;
            logText.overflowMode = TextOverflowModes.Overflow; // 确保文本不会被截断
            logText.enableWordWrapping = true;                 // 启用自动换行
        }

        // 计算基础高度（仅内边距，无文本时）
        baseHeight = padding * 2;
    }

    /// <summary>
    /// 绑定日志数据并计算高度
    /// </summary>
    public void SetData(LogData data, Color? specColor)
    {
        if (data == null || logText == null) return;
        string timeTxt = TimestampTool.FormatMillTime(data.timestamp, "HH:mm:ss.ff");
        // 设置日志内容和样式
        logText.text = $"<color=yellow>[{timeTxt}]</color> " + GetTypeStr(data.logType) + data.message;
        SetLogColor(data.logType, specColor);

        // 计算并更新高度
        UpdateItemHeight();
    }

    /// <summary>
    /// 根据日志类型设置颜色
    /// </summary>
    private void SetLogColor(LogMsgType logType, Color? specColor = null)
    {
        switch (logType)
        {
            case LogMsgType.Info:
                logText.color = infoColor;
                break;
            case LogMsgType.Warn:
                logText.color = warnColor;
                break;
            case LogMsgType.Error:
                logText.color = errorColor;
                break;
            case LogMsgType.Echo:
                logText.color = echoColor;
                break;
            case LogMsgType.Return:
                logText.color = returnColor;
                break;
            case LogMsgType.Spec:
                if (specColor == null)
                    logText.color = Color.magenta;
                else
                    logText.color = specColor.Value;
                break;
        }
    }
    private string GetTypeStr(LogMsgType logType)
    {

        return logType switch
        {
            LogMsgType.Info => "Info: ",
            LogMsgType.Warn => "Warn: ",
            LogMsgType.Error => "Error: ",
            LogMsgType.Echo => "Echo: ",
            LogMsgType.Return => "Return: ",
            LogMsgType.Spec => "",
            _ => "Info: ",
        };
    }

    /// <summary>
    /// 计算文本高度并更新Item高度
    /// </summary>
    private void UpdateItemHeight()
    {
        // 强制刷新TMP布局计算
        logText.ForceMeshUpdate();

        // TMP的preferredHeight已经包含了所有文本行的总高度
        float textHeight = logText.preferredHeight;

        // 总高度 = 文本高度 + 上下内边距
        float totalHeight = textHeight + baseHeight;

        // 确保最小高度（避免内容为空时高度为0）
        totalHeight = Mathf.Max(totalHeight, baseHeight + fontSize);

        // 更新Item高度（影响虚拟列表布局）
        SetItemHeight(totalHeight);
    }
    public void SetAnchorPosY(float y)
    {
        RT.anchoredPosition = new Vector2(RT.anchoredPosition.x, y);
    }

    /// <summary>
    /// 当Item被复用时重置状态
    /// </summary>
    public void OnRecycle()
    {
        if (logText != null)
        {
            logText.text = string.Empty;
            logText.color = infoColor;
        }
    }

    protected void SetItemHeight(float height)
    {
        if (RT != null)
        {
            var size = RT.sizeDelta;
            size.y = height;
            RT.sizeDelta = size;
        }

        // 通知虚拟列表高度已变更
        OnHeightChanged?.Invoke(height);
    }
}
