using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 命令行交互核心类
public class CommandLineCtrl : MonoBehaviour, IPointerClickHandler
{

    [Header("引用")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private SuggestionBox suggestionTips; // 用于显示补全建议

    [Header("配置")]
    [SerializeField] private bool isLogInUnityConsole = true;
    [SerializeField] private Color focusColor = Color.gray;
    [SerializeField] private Color normalColor = Color.blue;
    [SerializeField] private Image focusBg;
    private CommandLibrary _commandLibrary;
    public List<string> CmdHistory => _commandHistory;
    private List<string> _commandHistory = new List<string>();
    private int _historyIndex = -1;
    private string _currentInputBeforeHistory;
    private GameConsole Console => UIManager.Ins.console;
    private void Awake()
    {
        if (inputField == null)
            inputField = GetComponent<TMP_InputField>();
        _commandLibrary = new(this);
        suggestionTips.Hide();

        // 初始化输入事件
        inputField.onSubmit.AddListener(OnCommandSubmit);
        inputField.onValueChanged.AddListener(OnInputValueChanged);
        inputField.onSelect.AddListener(OnFocusInput);
        inputField.onDeselect.AddListener(OnCancelFocusInput);
        focusBg.color = normalColor;
        // 注册默认命令
    }
    private void OnFocusInput(string str)
    {
        focusBg.color = focusColor;
        if (!string.IsNullOrEmpty(str))
            suggestionTips.Show();
    }
    private void OnCancelFocusInput(string str)
    {
        focusBg.color = normalColor;
        suggestionTips.Hide();
    }
    private void OnEnable()
    {
        inputField.ActivateInputField();
        inputField.text = "";
    }
    void OnDisable()
    {
        inputField.DeactivateInputField();
        suggestionTips.Clear();
    }

    // 注册默认命令


    // 处理命令提交
    private void OnCommandSubmit(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            inputField.text = "";
            return;
        }

        // 首字符大写 + 剩余字符保持不变
        input = char.ToUpper(input[0]) + input[1..];
        // 添加到历史记录
        _commandHistory.Add(input);
        _historyIndex = -1;

        // 显示输入的命令
        Console.AddLog($"> {input}", LogMsgType.Echo);

        // 解析并执行命令
        ParseAndExecuteCommand(input);

        // 清空输入框
        inputField.text = "";
        inputField.ActivateInputField();
        suggestionTips.Clear();
        suggestionTips.Hide();
    }

    // 输入值变化时处理
    private void OnInputValueChanged(string input)
    {
        // 空输入不处理
        if (string.IsNullOrEmpty(input))
        {
            suggestionTips.Clear();
            suggestionTips.Hide();
            return;
        }

        // 分割命令
        (int wordIdx, List<string> parts) = GetSplitWords(inputField.text);
        string currentPart = wordIdx > parts.Count || wordIdx <= 0 ? "" : parts[wordIdx - 1];
        // 如果是第一个部分且不是空格结尾，尝试补全命令
        if (wordIdx == 1)
        {
            var matches = _commandLibrary.FindMatchingCommands(currentPart);

            suggestionTips.Show();
            suggestionTips.SetList(matches);
        }
        else if (wordIdx > 1)
        {
            CommandInfo info = _commandLibrary.GetCommandInfo(parts[0]);
            if (info == null)
            {
                suggestionTips.Hide();
                suggestionTips.Clear();
                return;
            }
            CommandArgument argInfo = info.Arguments.Count > wordIdx - 2 ? info.Arguments[wordIdx - 2] : null;
            if (argInfo == null)
            {
                suggestionTips.Hide();
                suggestionTips.Clear();
                return;
            }
            var matches = _commandLibrary.FindMatchingArgs(argInfo.Name, currentPart);
            suggestionTips.Show();
            suggestionTips.SetList(matches.res, matches.onlyTips);

        }
        else
        {
            suggestionTips.Clear();
            suggestionTips.Hide();
        }
    }

    // 处理自动补全
    private void HandleAutoCompletion()
    {
        if (suggestionTips.Matches.Count == 0 || suggestionTips.onlyTips)
            return;
        (int currentIndex, List<string> inputParts) = GetSplitWords(inputField.text);
        string lastMatch = suggestionTips.Matches[^1];
        if (currentIndex == 0 || currentIndex > inputParts.Count)
            return;
        inputParts[currentIndex - 1] = lastMatch;
        string newText = string.Join(" ", inputParts);
        inputField.text = newText;
        suggestionTips.Clear();
        suggestionTips.Hide();
        inputField.caretPosition = newText.Length;

    }

    // 获取光标所在的单词索引（从1开始）
    private (int, List<string>) GetSplitWords(string str, char split = ' ')
    {
        char[] splits = new[] { ' ', split };
        List<string> parts = str.Split(splits).ToList();

        int caretPos = Mathf.Clamp(inputField.caretPosition, 0, inputField.text.Length);

        // 获取光标前的文本并分割为单词
        string[] wordsBefore = str.Substring(0, caretPos)
            .Split(splits, System.StringSplitOptions.RemoveEmptyEntries);
        int wordCount = wordsBefore.Length;

        // 检查光标是否在空格后且前面有单词
        bool isAfterSpace = caretPos > 0 && caretPos <= str.Length
            && char.IsWhiteSpace(str[caretPos - 1])
            && (caretPos == str.Length || !char.IsWhiteSpace(str[caretPos]));

        // 计算最终索引
        if (isAfterSpace && wordCount > 0)
            return (wordCount + 1, parts);

        // 处理光标在文本开头但后面有单词的情况
        if (wordCount == 0)
            return (str.Split(splits, System.StringSplitOptions.RemoveEmptyEntries).Length > 0 ? 1 : 0, parts);

        return (wordCount, parts);
    }
    // 解析并执行命令
    private void ParseAndExecuteCommand(string input)
    {
        // 分割命令和参数（支持带引号的参数）
        var parts = SplitCommand(input);
        if (parts.Count == 0) return;

        string command = parts[0];
        var args = parts.Skip(1).ToArray();

        // 检查命令是否存在
        if (!_commandLibrary.CommandExists(command))
        {
            Console.AddLog($"未知命令 '{command}'，输入 'help' 查看所有命令", LogMsgType.Error);
            return;
        }

        // 获取命令信息
        var commandInfo = _commandLibrary.GetCommandInfo(command);

        // 检查参数数量
        int requiredArgsCount = commandInfo.Arguments.Count(a => a.IsRequired);
        if (args.Length < requiredArgsCount)
        {
            Console.AddLog($"命令 '{command}' 需要至少 {requiredArgsCount} 个参数", LogMsgType.Error);
            Console.AddLog($"用法：{command} " + string.Join(" ", commandInfo.Arguments.Select(a => $"<{(a.IsRequired ? "" : "[")}{a.Name}{(a.IsRequired ? "" : "]")}>")), LogMsgType.Return);
            return;
        }

        // 执行命令
        _commandLibrary.ExecuteCommand(command, args);
    }

    // 分割命令（支持带引号的参数）
    private List<string> SplitCommand(string input)
    {
        var parts = new List<string>();
        string currentPart = "";
        bool inQuotes = false;
        char quoteChar = '\0';

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // 处理引号
            if (c == '"' || c == '\'')
            {
                if (inQuotes && c == quoteChar)
                {
                    inQuotes = false;
                    quoteChar = '\0';
                }
                else if (!inQuotes)
                {
                    inQuotes = true;
                    quoteChar = c;
                }
                else
                {
                    currentPart += c;
                }
                continue;
            }

            // 处理空格
            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (!string.IsNullOrEmpty(currentPart))
                {
                    parts.Add(currentPart);
                    currentPart = "";
                }
                continue;
            }

            currentPart += c;
        }

        // 添加最后一部分
        if (!string.IsNullOrEmpty(currentPart))
        {
            parts.Add(currentPart);
        }

        return parts;
    }

    // 处理键盘事件（上下箭头浏览历史）
    private void Update()
    {
        if (inputField.isFocused)
        {

            // 上箭头：上一条历史
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (_commandHistory.Count == 0) return;

                if (_historyIndex == -1)
                {
                    _currentInputBeforeHistory = inputField.text;
                    _historyIndex = _commandHistory.Count - 1;
                }
                else if (_historyIndex > 0)
                {
                    _historyIndex--;
                }

                inputField.text = _commandHistory[_historyIndex];
                inputField.caretPosition = inputField.text.Length;
            }
            // 下箭头：下一条历史
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_historyIndex == -1) return;

                if (_historyIndex < _commandHistory.Count - 1)
                {
                    _historyIndex++;
                    inputField.text = _commandHistory[_historyIndex];
                }
                else
                {
                    _historyIndex = -1;
                    inputField.text = _currentInputBeforeHistory;
                }

                inputField.caretPosition = inputField.text.Length;
            }
            // Tab键：手动触发补全
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                HandleAutoCompletion();
            }
        }
    }

    // 点击时激活输入框
    public void OnPointerClick(PointerEventData eventData)
    {
        inputField.ActivateInputField();
    }
}

