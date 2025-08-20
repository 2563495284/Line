using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System;
using System.Globalization;

/// <summary>
/// 卡牌描述系统 - 处理动态数值和高亮显示
/// </summary>
public static class CardDescriptionSystem
{
    // 属性标记的正则表达式模式
    private static readonly Dictionary<EPlayerAttributeType, string> AttributePatterns = new Dictionary<EPlayerAttributeType, string>
    {
        { EPlayerAttributeType.Charisma, @"\{魅力\}" },
        { EPlayerAttributeType.Courage, @"\{勇气\}" },
        { EPlayerAttributeType.Wisdom, @"\{智慧\}" },
        { EPlayerAttributeType.Social, @"\{社交\}" },
        { EPlayerAttributeType.Patience, @"\{耐心\}" },
        { EPlayerAttributeType.Calmness, @"\{冷静\}" },
        { EPlayerAttributeType.Fanaticism, @"\{狂热\}" }
    };

    // 数值+属性组合的正则表达式模式（如：1{魅力}%）
    private static readonly string NumberAttributePattern = @"(\d+(?:\.\d+)?)\{([^}]+)\}%?";

    // 纯数值的正则表达式模式（用于识别基础数值）
    private static readonly string NumberPattern = @"\d+(?:\.\d+)?%?";

    // 属性名称映射
    private static readonly Dictionary<EPlayerAttributeType, string> AttributeNames = new Dictionary<EPlayerAttributeType, string>
    {
        { EPlayerAttributeType.Charisma, "魅力" },
        { EPlayerAttributeType.Courage, "勇气" },
        { EPlayerAttributeType.Wisdom   , "智慧" },
        { EPlayerAttributeType.Social, "社交" },
        { EPlayerAttributeType.Patience, "耐心" },
        { EPlayerAttributeType.Calmness, "冷静" },
        { EPlayerAttributeType.Fanaticism, "狂热" }
    };

    /// <summary>
    /// 卡牌描述解析结果
    /// </summary>
    public class CardDescriptionResult
    {
        public string processedDescription;           // 处理后的描述文本
        public string richTextDescription;          // 富文本格式的描述（用于显示）
        public List<EPlayerAttributeType> referencedAttributes; // 引用的属性类型
        public Dictionary<EPlayerAttributeType, float> attributeValues; // 属性对应的数值
    }

    /// <summary>
    /// 处理卡牌描述
    /// </summary>
    /// <param name="originalDescription">原始描述</param>
    /// <param name="card">卡牌数据</param>
    /// <returns>处理结果</returns>
    public static CardDescriptionResult ProcessCardDescription(string originalDescription, Card card)
    {
        var result = new CardDescriptionResult
        {
            processedDescription = originalDescription,
            richTextDescription = originalDescription,
            referencedAttributes = new List<EPlayerAttributeType>(),
            attributeValues = new Dictionary<EPlayerAttributeType, float>()
        };

        if (string.IsNullOrEmpty(originalDescription))
            return result;

        var playerAttributeSystem = PlayerAttributeSystem.Instance;
        if (playerAttributeSystem == null)
            return result;

        string processedText = originalDescription;
        string richText = originalDescription;

        // 处理数值+属性组合模式（如：1{魅力}%）
        var numberAttributeMatches = Regex.Matches(processedText, NumberAttributePattern);
        foreach (Match match in numberAttributeMatches)
        {
            string fullMatch = match.Groups[0].Value; // 完整匹配（如：1{魅力}%）
            float baseValue = float.Parse(match.Groups[1].Value); // 基础数值（如：1）
            string attributeName = match.Groups[2].Value; // 属性名称（如：魅力）

            // 查找对应的属性类型
            EPlayerAttributeType? attributeType = GetAttributeTypeByName(attributeName);
            if (attributeType.HasValue)
            {
                // 获取属性值
                float attributeValue = playerAttributeSystem.GetAttributeValue(attributeType.Value);

                // 计算属性影响系数（如魅力3 = 30%影响）
                float influenceMultiplier = CalculateInfluenceMultiplier(attributeType.Value, attributeValue);

                // 计算最终数值：基础值 × (1 + 影响系数)
                float finalValue = baseValue * (1f + influenceMultiplier);

                // 记录引用的属性
                if (!result.referencedAttributes.Contains(attributeType.Value))
                {
                    result.referencedAttributes.Add(attributeType.Value);
                }
                result.attributeValues[attributeType.Value] = finalValue;

                // 提取百分号（如果有的话）
                string percentSign = fullMatch.EndsWith("%") ? "%" : "";
                string basePattern = Regex.Escape(fullMatch);

                // 替换普通文本中的数值
                processedText = Regex.Replace(processedText, basePattern, $"{finalValue:F1}{percentSign}");

                // 替换富文本中的数值（添加高亮）
                string highlightedValue = $"<color=#FFD700><b>{finalValue:F1}</b></color>{percentSign}";
                richText = Regex.Replace(richText, basePattern, highlightedValue);
            }
        }

        // 处理纯属性标记（如：{魅力}）- 只高亮属性名称，不替换数值
        foreach (var kvp in AttributePatterns)
        {
            var attributeType = kvp.Key;
            var pattern = kvp.Value;
            var attributeName = AttributeNames[attributeType];

            if (Regex.IsMatch(processedText, pattern))
            {
                // 获取属性值
                float attributeValue = playerAttributeSystem.GetAttributeValue(attributeType);

                // 记录引用的属性（用于显示提示框）
                if (!result.referencedAttributes.Contains(attributeType))
                {
                    result.referencedAttributes.Add(attributeType);
                }
                result.attributeValues[attributeType] = attributeValue;

                // 对于纯属性标记，只在富文本中高亮属性名称，去掉大括号
                // 普通文本保持原样：{耐心} → {耐心}
                // 富文本高亮显示：{耐心} → <color=#87CEEB>耐心</color>
                string highlightedAttribute = $"<color=#FF0000>{attributeName}</color>";
                richText = Regex.Replace(richText, pattern, highlightedAttribute);
            }
        }

        result.processedDescription = processedText;
        result.richTextDescription = richText;

        // 调试信息
        Debug.Log($"[CardDescriptionSystem] 原始描述: '{originalDescription}'");
        Debug.Log($"[CardDescriptionSystem] 处理后描述: '{processedText}'");
        Debug.Log($"[CardDescriptionSystem] 富文本描述: '{richText}'");
        Debug.Log($"[CardDescriptionSystem] 引用属性数量: {result.referencedAttributes.Count}");

        return result;
    }

    /// <summary>
    /// 根据属性名称获取属性类型
    /// </summary>
    private static EPlayerAttributeType? GetAttributeTypeByName(string attributeName)
    {
        foreach (var kvp in AttributeNames)
        {
            if (kvp.Value == attributeName)
            {
                return kvp.Key;
            }
        }
        return null;
    }

    /// <summary>
    /// 计算属性影响系数（用于乘法计算）
    /// </summary>
    private static float CalculateInfluenceMultiplier(EPlayerAttributeType attributeType, float attributeValue)
    {
        switch (attributeType)
        {
            case EPlayerAttributeType.Charisma:
                // 魅力影响：每点魅力增加10%效果
                return attributeValue * 0.1f; // 3点魅力 = 0.3 (30%加成)

            case EPlayerAttributeType.Courage:
                // 勇气影响：每点勇气增加10%效果
                return attributeValue * 0.1f;

            case EPlayerAttributeType.Wisdom:
                // 智慧影响：每点智慧增加10%效果
                return attributeValue * 0.1f;

            case EPlayerAttributeType.Social:
                // 社交影响：每点社交增加10%效果
                return attributeValue * 0.1f;

            case EPlayerAttributeType.Patience:
                // 耐心影响：每点耐心增加10%效果
                return attributeValue * 0.1f;

            case EPlayerAttributeType.Calmness:
                // 冷静影响：每点冷静增加10%效果
                return attributeValue * 0.1f;

            case EPlayerAttributeType.Fanaticism:
                // 狂热影响：每点狂热增加10%效果
                return attributeValue * 0.1f;

            default:
                return 0f;
        }
    }

    /// <summary>
    /// 计算属性对效果的实际影响数值（保持向后兼容）
    /// </summary>
    private static float CalculateEffectValue(EPlayerAttributeType attributeType, float attributeValue, Card card)
    {
        switch (attributeType)
        {
            case EPlayerAttributeType.Charisma:
                // 魅力影响市场影响力，可能影响交易效果
                return attributeValue * 10f; // 每点魅力增加10%效果

            case EPlayerAttributeType.Courage:
                // 勇气影响交易数量
                return attributeValue; // 每点勇气增加1点交易数量

            case EPlayerAttributeType.Wisdom:
                // 智慧可能影响某些卡牌的效果
                return attributeValue;

            case EPlayerAttributeType.Social:
                // 社交可能影响某些卡牌效果
                return attributeValue;

            case EPlayerAttributeType.Patience:
                // 耐心可能影响持续效果
                return attributeValue;

            case EPlayerAttributeType.Calmness:
                // 冷静影响风险控制
                return attributeValue * 10f;

            case EPlayerAttributeType.Fanaticism:
                // 狂热影响激进操作
                return attributeValue * 10f;

            default:
                return attributeValue;
        }
    }

    /// <summary>
    /// 获取卡牌引用的所有属性类型
    /// </summary>
    public static List<EPlayerAttributeType> GetReferencedAttributes(string description)
    {
        var referencedAttributes = new List<EPlayerAttributeType>();

        if (string.IsNullOrEmpty(description))
            return referencedAttributes;

        // 检查数值+属性组合模式
        var numberAttributeMatches = Regex.Matches(description, NumberAttributePattern);
        foreach (Match match in numberAttributeMatches)
        {
            string attributeName = match.Groups[2].Value;
            EPlayerAttributeType? attributeType = GetAttributeTypeByName(attributeName);
            if (attributeType.HasValue && !referencedAttributes.Contains(attributeType.Value))
            {
                referencedAttributes.Add(attributeType.Value);
            }
        }

        // 检查纯属性标记模式
        foreach (var kvp in AttributePatterns)
        {
            if (Regex.IsMatch(description, kvp.Value) && !referencedAttributes.Contains(kvp.Key))
            {
                referencedAttributes.Add(kvp.Key);
            }
        }

        return referencedAttributes;
    }

    /// <summary>
    /// 检查描述中是否包含指定属性
    /// </summary>
    public static bool ContainsAttribute(string description, EPlayerAttributeType attributeType)
    {
        if (string.IsNullOrEmpty(description) || !AttributePatterns.ContainsKey(attributeType))
            return false;

        return Regex.IsMatch(description, AttributePatterns[attributeType]);
    }

    /// <summary>
    /// 获取属性在描述中的所有匹配位置
    /// </summary>
    public static List<int> GetAttributePositions(string description, EPlayerAttributeType attributeType)
    {
        var positions = new List<int>();

        if (string.IsNullOrEmpty(description) || !AttributePatterns.ContainsKey(attributeType))
            return positions;

        var matches = Regex.Matches(description, AttributePatterns[attributeType]);
        foreach (Match match in matches)
        {
            positions.Add(match.Index);
        }

        return positions;
    }
}
