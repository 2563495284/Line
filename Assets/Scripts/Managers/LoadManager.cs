using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// 加载进度参数
/// </summary>
public class LoadProgressEventArgs : EventArgs
{
    public string FolderName { get; }       // 当前加载的文件夹
    public string CurrentResource { get; }  // 当前加载的资源
    public float Progress { get; }          // 整体进度(0-1)
    public int LoadedCount { get; }         // 已加载数量
    public int TotalCount { get; }          // 总数量

    public LoadProgressEventArgs(string folderName, string currentResource,
                                float progress, int loadedCount, int totalCount)
    {
        FolderName = folderName;
        CurrentResource = currentResource;
        Progress = progress;
        LoadedCount = loadedCount;
        TotalCount = totalCount;
    }
}

/// <summary>
/// 加载完成参数
/// </summary>
public class LoadCompleteEventArgs : EventArgs
{
    public string FolderName { get; }           // 已加载的文件夹
    public int LoadedCount { get; }             // 成功加载数量
    public int TotalCount { get; }              // 总数量
    public bool IsSuccess { get; }              // 是否全部成功

    public LoadCompleteEventArgs(string folderName, int loadedCount, int totalCount, bool isSuccess)
    {
        FolderName = folderName;
        LoadedCount = loadedCount;
        TotalCount = totalCount;
        IsSuccess = isSuccess;
    }
}

/// <summary>
/// 资源加载管理器 - 按Resources下一级文件夹为单位加载
/// </summary>
public class LoadManager : Singleton<LoadManager>
{
    // 资源存储字典: 一级文件夹名 -> (资源路径 -> 资源对象)
    private Dictionary<string, Dictionary<string, UnityEngine.Object>> _resourceCache = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();
    public static List<Type> GetAllSubCls(Type t)
    {
        // 获取当前应用程序域中所有已加载的程序集
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 存储所有找到的子类
        List<Type> subclasses = new List<Type>();

        foreach (Assembly assembly in assemblies)
        {
            try
            {
                // 获取程序集中的所有类型
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    // 检查类型是否是类、不是抽象类、并且是LevelSystem的子类
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(t))
                    {
                        subclasses.Add(type);
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 处理类型加载异常
                foreach (Type type in ex.Types)
                {
                    if (type != null && type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(LevelSystem)))
                    {
                        subclasses.Add(type);
                    }
                }
            }
            catch (Exception)
            {
                // 处理其他可能的异常
                continue;
            }
        }

        return subclasses;
    }

    /// <summary>
    /// 按一级文件夹名称异步加载整个文件夹资源
    /// </summary>
    /// <param name="UIKey">Resources下的一级文件夹名称</param>
    /// <returns>协程迭代器</returns>
    public IEnumerator LoadAsync(string UIKey, Action<LoadProgressEventArgs> onProgress = null, Action<LoadCompleteEventArgs> onComplete = null)
    {
        // 检查文件夹是否已加载
        if (_resourceCache.ContainsKey(UIKey))
        {
            onComplete?.Invoke(new LoadCompleteEventArgs(
                UIKey,
                _resourceCache[UIKey].Count,
                _resourceCache[UIKey].Count,
                true));
            yield break;
        }

        // 获取该文件夹下的所有资源路径(从ResPath类中获取)
        var resourcePaths = GetResourcePathsForFolder(UIKey);
        if (resourcePaths == null || resourcePaths.Count == 0)
        {
            Debug.LogWarning($"文件夹 {UIKey} 没有找到可加载的资源路径");
            onComplete?.Invoke(new LoadCompleteEventArgs(UIKey, 0, 0, false));
            yield break;
        }

        // 初始化该文件夹的资源字典
        var folderResources = new Dictionary<string, UnityEngine.Object>();
        _resourceCache[UIKey] = folderResources;

        int totalCount = resourcePaths.Count;
        int loadedCount = 0;
        bool allSuccess = true;

        // 逐个异步加载资源
        foreach (var path in resourcePaths)
        {
            // 异步加载资源
            var request = Resources.LoadAsync(path);
            while (!request.isDone)
            {
                // 计算进度并触发进度事件
                float progress = (loadedCount + request.progress) / totalCount;
                onProgress?.Invoke(new LoadProgressEventArgs(
                    UIKey,
                    path,
                    progress,
                    loadedCount,
                    totalCount));
                yield return null;
            }

            // 处理加载结果
            if (request.asset != null)
            {
                folderResources[path] = request.asset;
                loadedCount++;
            }
            else
            {
                Debug.LogError($"资源加载失败: {path}");
                allSuccess = false;
            }

            // 触发完成单个资源的进度事件
            onProgress?.Invoke(new LoadProgressEventArgs(
                UIKey,
                path,
                (float)loadedCount / totalCount,
                loadedCount,
                totalCount));

            yield return null;
        }

        // 触发加载完成事件
        onComplete?.Invoke(new LoadCompleteEventArgs(
            UIKey,
            loadedCount,
            totalCount,
            allSuccess));
    }
    /// <summary>
    /// 使用反射从ResPath类中获取指定文件夹的所有资源路径
    /// </summary>
    /// <param name="folderName">一级文件夹名称</param>
    /// <returns>该文件夹下所有资源的路径集合</returns>
    private static List<string> GetResourcePathsForFolder(string folderName)
    {
        List<string> resourcePaths = new List<string>();

        try
        {
            // 获取ResPath类型
            Type resPathType = Type.GetType("ResPath, Assembly-CSharp");
            if (resPathType == null)
            {
                Debug.LogError("未找到ResPath类，请先执行Tools/BuildResourcesPath生成");
                return resourcePaths;
            }

            // 获取指定的内部静态类（如ResPath.UI）
            Type folderType = resPathType.GetNestedType(folderName, BindingFlags.Public | BindingFlags.Static);
            if (folderType == null)
            {
                Debug.LogError($"ResPath中未找到{folderName}内部类");
                return resourcePaths;
            }

            // 获取所有公共静态字符串字段（资源路径常量）
            FieldInfo[] fieldInfos = folderType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var field in fieldInfos)
            {
                // 排除key字段，只获取资源路径字段
                if (field.FieldType == typeof(string) && field.Name != "Key")
                {
                    string pathValue = field.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(pathValue))
                    {
                        resourcePaths.Add(pathValue);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"反射获取资源路径失败: {ex.Message}");
        }

        return resourcePaths;
    }

    /// <summary>
    /// 同步获取资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="folderName">一级文件夹名称</param>
    /// <param name="resourceKey">资源路径(相对于Resources)</param>
    /// <returns>资源对象或null</returns>
    public T GetRes<T>(string folderName, string resourceKey) where T : UnityEngine.Object
    {
        if (_resourceCache.TryGetValue(folderName, out var folderResources) &&
            folderResources.TryGetValue(resourceKey, out var resource))
        {
            return resource as T;
        }

        Debug.LogWarning($"未找到资源: 文件夹={folderName}, 路径={resourceKey}");
        return null;
    }

    public T GetResByName<T>(string folderName, string name) where T : UnityEngine.Object
    {
        if (_resourceCache.TryGetValue(folderName, out var folderResources))
        {
            List<string> keys = folderResources.Keys.ToList();
            int idx = keys.FindIndex(e => e.EndsWith(name));
            if (idx >= 0)
                return folderResources[keys[idx]] as T;
        }
        Debug.LogWarning($"未找到资源: 文件夹={folderName}, 文件名={name}");
        return null;
    }

    /// <summary>
    /// 卸载指定文件夹的资源
    /// </summary>
    /// <param name="folderName">文件夹名称</param>
    /// <param name="unloadUnusedAssets">是否立即卸载未使用资源</param>
    public void UnloadFolder(string folderName, bool unloadUnusedAssets = false)
    {
        if (_resourceCache.TryGetValue(folderName, out var folderResources))
        {
            folderResources.Clear();
            _resourceCache.Remove(folderName);

            if (unloadUnusedAssets)
            {
                Resources.UnloadUnusedAssets();
            }
        }
    }

    /// <summary>
    /// 检查文件夹是否已加载
    /// </summary>
    public bool IsLoaded(string folderName)
    {
        return _resourceCache.ContainsKey(folderName) && _resourceCache[folderName].Count > 0;
    }

    /// <summary>
    /// 获取文件夹已加载资源数量
    /// </summary>
    public int GetLoadedCount(string folderName)
    {
        return _resourceCache.TryGetValue(folderName, out var folderResources) ? folderResources.Count : 0;
    }
}
