using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using GameConfig;

/// <summary>
/// 卡牌Utils
/// </summary>
public static class CardUtils
{
    // 属性标记的正则表达式模式
    private static readonly Dictionary<AttrType, string> AttributePatterns = new Dictionary<AttrType, string>
    {
        { AttrType.Charisma, @"\[魅力\]" },
        { AttrType.Courage, @"\[勇气\]" },
        { AttrType.Wisdom, @"\[智慧\]" },
        { AttrType.Social, @"\[社交\]" },
        { AttrType.Calmness, @"\[冷静\]" },
        { AttrType.Fanaticism, @"\[狂热\]" }
    };

    // 数值+属性组合的正则表达式模式（如：1{魅力}%）
    private static readonly string NumberAttributePattern = @"(\d+(?:\.\d+)?)\{([^}]+)\}?";

    // 属性名称映射
    private static readonly Dictionary<AttrType, string> AttributeNames = new Dictionary<AttrType, string>
    {
        { AttrType.Charisma, "魅力" },
        { AttrType.Courage, "勇气" },
        { AttrType.Wisdom   , "智慧" },
        { AttrType.Social, "社交" },
        { AttrType.Calmness, "冷静" },
        { AttrType.Fanaticism, "狂热" }
    };

    /// <summary>
    /// 卡牌描述解析结果
    /// </summary>
    public class CardDesc
    {
        public string processedDescription;           // 处理后的描述文本
        public string richTextDescription;          // 富文本格式的描述（用于显示）
        public List<AttrType> referencedAttributes; // 引用的属性类型
    }

    public static CardDesc ProcessCardDescription(string originalDescription)
    {
        var result = new CardDesc
        {
            processedDescription = originalDescription,
            richTextDescription = originalDescription,
            referencedAttributes = new List<AttrType>()
        };

        if (string.IsNullOrEmpty(originalDescription))
            return result;

        var playerAttributeSystem = PlayerAttributeSystem.Ins;
        if (playerAttributeSystem == null)
            return result;

        string processedText = originalDescription;
        string richText = originalDescription;

        var numberAttributeMatches = Regex.Matches(processedText, NumberAttributePattern);
        foreach (Match match in numberAttributeMatches)
        {
            richText = ReplaceAttributeDesc(result, richText, match);
        }

        foreach (var kvp in AttributePatterns)
        {
            var attributeType = kvp.Key;
            var pattern = kvp.Value;
            var attributeName = Config.AttrConfig.Get(attributeType).Name;

            if (Regex.IsMatch(processedText, pattern))
            {
                // 获取属性值
                float attributeValue = playerAttributeSystem.GetAttributeValue(attributeType);

                // 记录引用的属性（用于显示提示框）
                if (!result.referencedAttributes.Contains(attributeType))
                {
                    result.referencedAttributes.Add(attributeType);
                }

                string highlightedAttribute = $"<color=#FF0000>{attributeName}</color>";
                richText = Regex.Replace(richText, pattern, highlightedAttribute);
            }
        }

        result.processedDescription = processedText;
        result.richTextDescription = richText;


        return result;
    }

    /// <summary>
    /// 根据属性名称获取属性类型
    /// </summary>
    private static AttrType? GetAttributeTypeByName(string attributeName)
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
    private static string ReplaceAttributeDesc(CardDesc result, string richText, Match match)
    {
        var playerAttributeSystem = PlayerAttributeSystem.Ins;
        string fullMatch = match.Groups[0].Value; // 完整匹配（如：1{魅力}）
        string attributeName = match.Groups[2].Value; // 属性名称（如：魅力）

        // 查找对应的属性类型
        AttrType? attributeType = GetAttributeTypeByName(attributeName);
        if (!attributeType.HasValue)
        {
            return richText;
        }
        switch (attributeType)
        {
            case AttrType.Courage:
                int baseValueCourage = int.Parse(match.Groups[1].Value); // 基础数值（如：1）
                // 勇气影响：每点勇气增加10%效果
                // 获取属性值
                int attributeValueCourage = (int)playerAttributeSystem.GetAttributeValue(attributeType.Value);


                int finalValueCourage = (int)math.floor(baseValueCourage * (1 + attributeValueCourage * 0.1f));

                string basePatternCourage = Regex.Escape(fullMatch);
                return Regex.Replace(richText, basePatternCourage, $"<color=#FF0000><b>{finalValueCourage}</b></color>");
            case AttrType.Charisma:
                float baseValueCharisma = float.Parse(match.Groups[1].Value); // 基础数值（如：1）
                // 魅力影响，每层10%
                // 获取属性值
                float attributeValueCharisma = playerAttributeSystem.GetAttributeValue(attributeType.Value);


                float finalValue = baseValueCharisma * (1 + attributeValueCharisma * 0.1f) * 100;
                string basePatternCharisma = Regex.Escape(fullMatch);
                return Regex.Replace(richText, basePatternCharisma, $"<color=#FF0000><b>{finalValue.ToString("F2")}%</b></color>");
            default:
                return "";
        }
    }

    /// <summary>
    /// 获取卡牌引用的所有属性类型
    /// </summary>
    public static List<AttrType> GetReferencedAttributes(string description)
    {
        var referencedAttributes = new List<AttrType>();

        if (string.IsNullOrEmpty(description))
            return referencedAttributes;

        // 检查数值+属性组合模式
        var numberAttributeMatches = Regex.Matches(description, NumberAttributePattern);
        foreach (Match match in numberAttributeMatches)
        {
            string attributeName = match.Groups[2].Value;
            AttrType? attributeType = GetAttributeTypeByName(attributeName);
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
    public static bool ContainsAttribute(string description, AttrType attributeType)
    {
        if (string.IsNullOrEmpty(description) || !AttributePatterns.ContainsKey(attributeType))
            return false;

        return Regex.IsMatch(description, AttributePatterns[attributeType]);
    }

    /// <summary>
    /// 获取属性在描述中的所有匹配位置
    /// </summary>
    public static List<int> GetAttributePositions(string description, AttrType attributeType)
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
