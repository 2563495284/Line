using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
//AI generate
public class ScriptReferenceScanner : EditorWindow
{
    private string folderAPath = "Assets/Scripts/A";
    private string folderBPath = "Assets/Scripts/B";
    private Dictionary<string, List<string>> referenceDetails = new Dictionary<string, List<string>>();
    private Vector2 scrollPosition;
    private bool showAdvancedOptions;
    private bool includeVariables = true;
    private bool includeComments = false;

    [MenuItem("Tools/Enhanced Script Reference Scanner")]
    public static void ShowWindow()
    {
        GetWindow<ScriptReferenceScanner>("增强版脚本引用扫描器");
    }

    private void OnGUI()
    {
        GUILayout.Label("增强版脚本引用扫描器", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 文件夹路径设置
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("A文件夹路径:", GUILayout.Width(100));
        folderAPath = EditorGUILayout.TextField(folderAPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
            folderAPath = GetFolderPath(folderAPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("B文件夹路径:", GUILayout.Width(100));
        folderBPath = EditorGUILayout.TextField(folderBPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
            folderBPath = GetFolderPath(folderBPath);
        EditorGUILayout.EndHorizontal();

        // 高级选项
        showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "高级选项");
        if (showAdvancedOptions)
        {
            EditorGUI.indentLevel++;
            includeVariables = EditorGUILayout.Toggle("检测变量引用", includeVariables);
            includeComments = EditorGUILayout.Toggle("包含注释中的引用", includeComments);
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(20);

        // 扫描按钮
        if (GUILayout.Button("开始扫描", GUILayout.Height(30)))
        {
            ScanReferences();
        }

        GUILayout.Space(10);

        // 显示结果
        GUILayout.Label("扫描结果:", EditorStyles.boldLabel);
        if (referenceDetails.Count > 0)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));
            foreach (var kvp in referenceDetails)
            {
                EditorGUILayout.LabelField($"> {kvp.Key}", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (var reference in kvp.Value)
                {
                    EditorGUILayout.LabelField($"- {reference}");
                }
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
            EditorGUILayout.EndScrollView();
            GUILayout.Label($"共找到 {referenceDetails.Count} 个脚本，引用了B文件夹中的元素");
        }
        else if (Event.current.type == EventType.Layout)
        {
            EditorGUILayout.LabelField("暂无结果，请点击扫描按钮");
        }
    }

    private string GetFolderPath(string currentPath)
    {
        string selectedPath = EditorUtility.OpenFolderPanel("选择文件夹", currentPath, "");
        if (!string.IsNullOrEmpty(selectedPath) && selectedPath.Contains(Application.dataPath))
        {
            return "Assets" + selectedPath.Substring(Application.dataPath.Length);
        }
        return currentPath;
    }

    private void ScanReferences()
    {
        referenceDetails.Clear();

        // 验证文件夹
        if (!Directory.Exists(folderAPath))
        {
            EditorUtility.DisplayDialog("错误", $"A文件夹不存在: {folderAPath}", "确定");
            return;
        }

        if (!Directory.Exists(folderBPath))
        {
            EditorUtility.DisplayDialog("错误", $"B文件夹不存在: {folderBPath}", "确定");
            return;
        }

        // 获取B文件夹中的所有可引用元素（类、结构体、枚举、公共静态变量）
        var bFolderElements = GetReferencableElementsInFolder(folderBPath);

        if (bFolderElements.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "B文件夹中未找到可引用的元素", "确定");
            return;
        }

        // 扫描A文件夹中的脚本
        string[] aFolderScripts = Directory.GetFiles(folderAPath, "*.cs", SearchOption.AllDirectories);
        int processed = 0;

        foreach (string scriptPath in aFolderScripts)
        {
            EditorUtility.DisplayProgressBar("扫描中",
                $"正在处理: {Path.GetFileName(scriptPath)} ({processed}/{aFolderScripts.Length})",
                (float)processed / aFolderScripts.Length);

            var references = CheckScriptReferences(scriptPath, bFolderElements);
            if (references.Count > 0)
            {
                string relativePath = scriptPath.Replace(Application.dataPath, "Assets");
                referenceDetails[relativePath] = references;
            }

            processed++;
        }

        EditorUtility.ClearProgressBar();
        Repaint();
    }

    /// <summary>
    /// 获取B文件夹中所有可被引用的元素（类、结构体、枚举、公共静态变量）
    /// </summary>
    private Dictionary<string, string> GetReferencableElementsInFolder(string folderPath)
    {
        var elements = new Dictionary<string, string>(); // 键:元素名, 值:元素类型描述
        string[] scripts = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

        foreach (string scriptPath in scripts)
        {
            string scriptContent = File.ReadAllText(scriptPath);
            // 移除注释（可选）
            if (!includeComments)
                scriptContent = RemoveComments(scriptContent);

            // 提取类
            var classMatches = Regex.Matches(scriptContent, @"(public|internal)\s+class\s+(\w+)\s*[:{]");
            foreach (Match match in classMatches)
                if (!elements.ContainsKey(match.Groups[2].Value))
                    elements[match.Groups[2].Value] = $"类 ({match.Groups[1].Value})";

            // 提取结构体
            var structMatches = Regex.Matches(scriptContent, @"(public|internal)\s+struct\s+(\w+)\s*[:{]");
            foreach (Match match in structMatches)
                if (!elements.ContainsKey(match.Groups[2].Value))
                    elements[match.Groups[2].Value] = $"结构体 ({match.Groups[1].Value})";

            // 提取枚举
            var enumMatches = Regex.Matches(scriptContent, @"(public|internal)\s+enum\s+(\w+)\s*[:{]");
            foreach (Match match in enumMatches)
                if (!elements.ContainsKey(match.Groups[2].Value))
                    elements[match.Groups[2].Value] = $"枚举 ({match.Groups[1].Value})";

            // 提取公共静态变量
            if (includeVariables)
            {
                var variableMatches = Regex.Matches(scriptContent,
                    @"(public|internal)\s+static\s+\w+\s+(\w+)\s*[=;]");
                foreach (Match match in variableMatches)
                    if (!elements.ContainsKey(match.Groups[2].Value))
                        elements[match.Groups[2].Value] = $"静态变量 ({match.Groups[1].Value})";
            }
        }

        return elements;
    }

    /// <summary>
    /// 检查脚本是否引用了B文件夹中的元素，并返回引用详情
    /// </summary>
    private List<string> CheckScriptReferences(string scriptPath, Dictionary<string, string> bElements)
    {
        var references = new List<string>();
        string scriptContent = File.ReadAllText(scriptPath);

        if (!includeComments)
            scriptContent = RemoveComments(scriptContent);

        foreach (var element in bElements)
        {
            string elementName = element.Key;
            string elementType = element.Value;

            // 匹配作为类型或变量使用的情况（单词边界匹配避免部分匹配）
            if (Regex.IsMatch(scriptContent, $@"\b{Regex.Escape(elementName)}\b"))
            {
                references.Add($"{elementType}: {elementName}");
            }
        }

        return references.Distinct().ToList();
    }

    /// <summary>
    /// 移除代码中的注释，避免误检测
    /// </summary>
    private string RemoveComments(string code)
    {
        // 移除单行注释
        code = Regex.Replace(code, @"//.*$", "", RegexOptions.Multiline);
        // 移除多行注释
        code = Regex.Replace(code, @"/\*.*?\*/", "", RegexOptions.Singleline);
        return code;
    }
}
