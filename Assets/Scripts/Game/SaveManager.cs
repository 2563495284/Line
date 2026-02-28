using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 存档管理器 —— 负责全局存档的读取、保存、删除。
/// 使用 JsonUtility 序列化到 Application.persistentDataPath。
/// </summary>
public class SaveManager : PersistentSingleton<SaveManager>
{

    private const string SAVE_FILE_NAME = "global_save.json";

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    // ── 查询 ───────────────────────────────────────────────────────────────────

    public bool HasSave() => File.Exists(SaveFilePath);

    // ── 读取 ───────────────────────────────────────────────────────────────────

    /// <summary>读取存档，失败或不存在时返回 null</summary>
    public GlobalSaveData Load()
    {
        if (!HasSave())
        {
            Debug.LogWarning("[SaveManager] 存档文件不存在");
            return null;
        }

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            var data = JsonUtility.FromJson<GlobalSaveData>(json);
            data.PostDeserialize();
            Debug.Log($"[SaveManager] 存档读取成功 ({SaveFilePath})");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 存档读取失败: {e.Message}");
            return null;
        }
    }

    // ── 保存 ───────────────────────────────────────────────────────────────────

    public void Save(GlobalSaveData data)
    {
        if (data == null)
        {
            Debug.LogError("[SaveManager] 尝试保存 null 数据");
            return;
        }

        try
        {
            data.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"[SaveManager] 存档保存成功 ({data.saveTime})");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 存档保存失败: {e.Message}");
        }
    }

    // ── 新建 ───────────────────────────────────────────────────────────────────

    /// <summary>创建默认新存档并立即写入磁盘</summary>
    public GlobalSaveData CreateNewSave()
    {
        var data = GlobalSaveData.CreateDefault();
        Save(data);
        Debug.Log("[SaveManager] 新存档已创建");
        return data;
    }

    // ── 删除 ───────────────────────────────────────────────────────────────────

    public void Delete()
    {
        if (!HasSave())
        {
            Debug.LogWarning("[SaveManager] 没有存档可删除");
            return;
        }

        try
        {
            File.Delete(SaveFilePath);
            Debug.Log("[SaveManager] 存档已删除");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 存档删除失败: {e.Message}");
        }
    }
}
