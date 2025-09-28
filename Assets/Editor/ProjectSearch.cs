using UnityEditor;
using UnityEngine;
using System;

[InitializeOnLoad]
public static class ProjectSearch
{
    static ProjectSearch()
    {
        // 注册快捷键处理
        EditorApplication.update += HandleShortcuts;
    }

    static void HandleShortcuts()
    {
        // 检测Ctrl+K快捷键（Windows/Linux）
        // 对于Mac，可使用 (Event.current.commandKey && Event.current.keyCode == KeyCode.K)
        if (Event.current != null && Event.current.type == EventType.KeyDown &&
            Event.current.control &&
            Event.current.keyCode == KeyCode.K)
        {
            // 激活Project窗口
            Type projectWindowType = Type.GetType("UnityEditor.ProjectWindow, UnityEditor");
            EditorWindow projectWindow = EditorWindow.GetWindow(projectWindowType);
            if (projectWindow != null)
            {
                projectWindow.Focus();
            }

            // 触发搜索框聚焦（模拟快捷键Ctrl+F）
            SimulateSearchFocus();

            // 标记事件为已使用，防止默认行为
            Event.current.Use();
        }
    }

    static void SimulateSearchFocus()
    {
        // 模拟按下Ctrl+F来聚焦搜索框
        var e = new Event { type = EventType.KeyDown, keyCode = KeyCode.F, control = true };
        EditorWindow.focusedWindow.SendEvent(e);
    }
}
