using UnityEngine;
using UnityEngine.UI;

public class FontTest : MonoBehaviour
{
    void Start()
    {
        TestFontLoading();
    }

    void TestFontLoading()
    {
        // 测试LegacyRuntime字体
        Font legacyFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (legacyFont != null)
        {
            Debug.Log("✅ LegacyRuntime.ttf 字体加载成功");
        }
        else
        {
            Debug.LogError("❌ LegacyRuntime.ttf 字体加载失败");
        }

        // 测试FontHelper
        if (FontHelper.GetDefaultFont() != null)
        {
            Debug.Log("✅ FontHelper 字体获取成功");
        }
        else
        {
            Debug.LogError("❌ FontHelper 字体获取失败");
        }
    }
}