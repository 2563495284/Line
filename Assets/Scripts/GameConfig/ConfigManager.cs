using System;
using UnityEngine;
using SimpleJSON;

/// <summary>
/// 游戏配置管理器 - 负责加载和管理由 Luban 生成的所有配置表
/// 使用方法：在游戏启动时调用 ConfigManager.Instance.Load()
/// 生成步骤：在终端中运行 DataTables/gen.sh 重新生成配置
/// </summary>
public class ConfigManager : MonoBehaviour
{
    private static ConfigManager _instance;
    public static ConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("ConfigManager");
                _instance = go.AddComponent<ConfigManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public cfg.Tables Tables { get; private set; }
    public bool IsLoaded { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    /// <summary>
    /// 加载所有配置表
    /// </summary>
    public void Load()
    {
        try
        {
            Tables = new cfg.Tables(LoadJson);
            IsLoaded = true;
            Debug.Log("[ConfigManager] 配置表加载成功");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ConfigManager] 配置表加载失败: {e.Message}\n" +
                           "请先运行 DataTables/gen.sh 生成配置文件\n" +
                           $"{e.StackTrace}");
        }
    }

    /// <summary>
    /// 从 Resources/Config 目录加载 JSON 配置文件
    /// JSON 文件由 DataTables/gen.sh 脚本生成并输出至 Assets/Resources/Config/
    /// </summary>
    private static JSONNode LoadJson(string fileName)
    {
        var textAsset = Resources.Load<TextAsset>($"Config/{fileName}");
        if (textAsset == null)
        {
            throw new Exception($"找不到配置文件: Resources/Config/{fileName}\n请先运行 DataTables/gen.sh 生成配置");
        }
        return JSON.Parse(textAsset.text);
    }
}
