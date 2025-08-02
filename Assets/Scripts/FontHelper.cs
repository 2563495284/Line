using UnityEngine;
using UnityEngine.UI;

public static class FontHelper
{
    private static Font _defaultFont;

    /// <summary>
    /// 获取默认字体，优先使用LegacyRuntime.ttf，如果失败则使用系统默认字体
    /// </summary>
    public static Font GetDefaultFont()
    {
        if (_defaultFont != null)
            return _defaultFont;

        // 尝试获取LegacyRuntime字体
        _defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 如果获取失败，使用系统默认字体
        if (_defaultFont == null)
        {
            _defaultFont = Font.CreateDynamicFontFromOSFont("Arial", 12);

            // 如果还是失败，使用Unity默认字体
            if (_defaultFont == null)
            {
                _defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
        }

        return _defaultFont;
    }

    /// <summary>
    /// 设置Text组件的字体
    /// </summary>
    public static void SetTextFont(Text text)
    {
        if (text != null)
        {
            text.font = GetDefaultFont();
        }
    }

    /// <summary>
    /// 创建Text组件并设置默认字体
    /// </summary>
    public static Text CreateText(GameObject parent, string textContent = "")
    {
        Text text = parent.AddComponent<Text>();
        text.text = textContent;
        text.font = GetDefaultFont();
        return text;
    }
}