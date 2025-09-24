using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

/// <summary>
/// 资源路径生成器 - 生成ResPath.cs文件
/// </summary>
public class ResourcePathBuilder : EditorWindow
{
    // 生成的脚本保存路径（相对于Assets目录）
    private const string outputScriptPath = "Scripts/Const/ResPath.cs";

    [MenuItem("Tools/Build Resource Paths")]
    private static void GenerateResPathFile()
    {
        // 查找Resources目录
        string resourcesPath = FindResourcesDirectory();
        if (string.IsNullOrEmpty(resourcesPath))
        {
            EditorUtility.DisplayDialog("错误", "未找到Resources目录", "确定");
            return;
        }

        // 获取Resources目录下的一级文件夹
        List<string> firstLevelDirs = GetFirstLevelDirectories(resourcesPath);
        if (firstLevelDirs.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "Resources目录下没有一级文件夹", "确定");
            return;
        }

        // 为每个一级文件夹收集其下所有资源路径
        Dictionary<string, List<string>> dirResources = new Dictionary<string, List<string>>();
        foreach (var dir in firstLevelDirs)
        {
            var resourcePaths = GetAllResourcePaths(dir, resourcesPath);
            dirResources[dir] = resourcePaths;
        }

        // 生成脚本内容
        string scriptContent = GenerateScriptContent(firstLevelDirs, dirResources);

        // 确保输出目录存在
        EnsureDirectoryExists(outputScriptPath);

        // 写入文件
        File.WriteAllText(Application.dataPath + "/" + outputScriptPath, scriptContent);

        // 刷新AssetDatabase
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("完成",
            $"已生成ResPath.cs，包含{firstLevelDirs.Count}个文件夹的路径定义", "确定");
    }

    /// <summary>
    /// 查找项目中的Resources目录
    /// </summary>
    private static string FindResourcesDirectory()
    {
        string[] resourceDirs = Directory.GetDirectories(Application.dataPath, "Resources", SearchOption.AllDirectories);
        return resourceDirs.Length > 0 ? resourceDirs[0] : null;
    }

    /// <summary>
    /// 获取Resources目录下的所有一级文件夹
    /// </summary>
    private static List<string> GetFirstLevelDirectories(string resourcesPath)
    {
        return Directory.GetDirectories(resourcesPath)
            .Select(dir => Path.GetFileName(dir))
            .ToList();
    }

    /// <summary>
    /// 获取指定目录下所有资源的相对路径
    /// </summary>
    private static List<string> GetAllResourcePaths(string directoryName, string resourcesRoot)
    {
        string fullDirPath = Path.Combine(resourcesRoot, directoryName);
        List<string> resourcePaths = new List<string>();

        // 查找所有非.meta文件
        string[] allFiles = Directory.GetFiles(fullDirPath, "*.*", SearchOption.AllDirectories)
            .Where(file => !file.EndsWith(".meta"))
            .ToArray();

        foreach (string file in allFiles)
        {
            // 计算相对于Resources的路径（不含扩展名）
            string relativePath = GetRelativePath(resourcesRoot, file);
            string resourcePath = Path.ChangeExtension(relativePath, null)
                .Replace("\\", "/"); // 统一使用正斜杠

            resourcePaths.Add(resourcePath);
        }

        return resourcePaths;
    }

    /// <summary>
    /// 生成ResPath.cs脚本内容
    /// </summary>
    private static string GenerateScriptContent(List<string> firstLevelDirs, Dictionary<string, List<string>> dirResources)
    {
        StringBuilder sb = new StringBuilder();

        // 脚本头部
        sb.AppendLine("// 此文件由ResourcePathBuilder自动生成，请勿手动修改");
        sb.AppendLine("public static class ResPath");
        sb.AppendLine("{");

        // 为每个一级文件夹生成内部静态类
        foreach (var dir in firstLevelDirs)
        {
            var resourcePaths = dirResources[dir];

            // 内部静态类定义
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {dir}文件夹资源路径");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    public static class {dir}");
            sb.AppendLine("    {");

            // 添加文件夹名称常量
            sb.AppendLine($"        /// <summary>");
            sb.AppendLine($"        /// 文件夹名称");
            sb.AppendLine($"        /// </summary>");
            sb.AppendLine($"        public static readonly string Key = \"{dir}\";");
            sb.AppendLine();

            // 添加所有资源路径数组
            sb.AppendLine($"        /// <summary>");
            sb.AppendLine($"        /// 所有资源路径数组");
            sb.AppendLine($"        /// </summary>");
            sb.AppendLine($"        public static readonly string[] AllPaths = new string[]");
            sb.AppendLine("        {");
            for (int i = 0; i < resourcePaths.Count; i++)
            {
                string path = resourcePaths[i];
                bool isLast = i == resourcePaths.Count - 1;
                sb.AppendLine($"            \"{path}\"{(isLast ? "" : ",")}");
            }
            sb.AppendLine("        };");
            sb.AppendLine();

            // 按子文件夹分组添加资源路径常量
            var groupedPaths = resourcePaths.GroupBy(path =>
                Path.GetDirectoryName(path).Replace("/", "."));

            foreach (var group in groupedPaths)
            {
                if (!string.IsNullOrEmpty(group.Key))
                {
                    sb.AppendLine($"        // {group.Key.Replace(".", "/")}相关资源");
                }

                foreach (var path in group)
                {
                    string fileName = Path.GetFileName(path);
                    string constName = GenerateValidConstantName(fileName);
                    sb.AppendLine($"        public const string {constName} = \"{path}\";");
                }
                sb.AppendLine();
            }

            // 内部类结束
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        // 主类结束
        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// 生成合法的常量名
    /// </summary>
    private static string GenerateValidConstantName(string fileName)
    {
        // 替换非法字符
        string name = System.Text.RegularExpressions.Regex.Replace(fileName, @"[^a-zA-Z0-9_]", "_");
        // 确保以字母开头
        if (name.Length > 0 && !char.IsLetter(name[0]))
        {
            name = "Res_" + name;
        }
        return name;
    }

    /// <summary>
    /// 获取相对路径
    /// </summary>
    private static string GetRelativePath(string basePath, string targetPath)
    {
        string normalizedBasePath = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        string normalizedTargetPath = Path.GetFullPath(targetPath);

        if (!normalizedTargetPath.StartsWith(normalizedBasePath))
        {
            return normalizedTargetPath;
        }

        return normalizedTargetPath.Substring(normalizedBasePath.Length);
    }

    /// <summary>
    /// 确保输出目录存在
    /// </summary>
    private static void EnsureDirectoryExists(string filePath)
    {
        string directoryPath = Path.GetDirectoryName(Application.dataPath + "/" + filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
}
