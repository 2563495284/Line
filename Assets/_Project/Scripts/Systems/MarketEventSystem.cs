using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EEventCardType
{
    Bull,      // 做多卡组
    Bear,      // 做空卡组
    Neutral    // 中立卡组
}

/// <summary>
/// 概率计算模式
/// </summary>
public enum ProbabilityMode
{
    Linear,        // 线性模式
    Exponential,   // 指数模式
    Stepped,       // 阶梯模式
    Curve          // 曲线模式（使用AnimationCurve）
}

/// <summary>
/// 概率阶梯配置
/// </summary>
[System.Serializable]
public class ProbabilityStep
{
    public float deviationThreshold;  // 偏离阈值
    public float probability;         // 对应概率

    public ProbabilityStep(float threshold, float prob)
    {
        deviationThreshold = threshold;
        probability = prob;
    }
}

/// <summary>
/// 单个事件类型的概率配置
/// </summary>
[System.Serializable]
public class EventProbabilityConfig
{
    [Header("指数模式")]
    public float exponentialBase = 2.0f;          // 指数底数
    public float minProbability = 0.05f;          // 最小概率
    public float maxProbability = 0.9f;           // 最大概率

    [Header("基础权重")]
    public float baseWeight = 0.3f;               // 基础权重（偏离为0时）

    public EventProbabilityConfig(float expBase, float minProb, float maxProb)
    {
        exponentialBase = expBase;
        minProbability = minProb;
        maxProbability = maxProb;
        baseWeight = minProb;
    }
}

/// <summary>
/// 阶梯模式配置
/// </summary>
[System.Serializable]
public class ProbabilityStepConfig
{
    public ProbabilityStep[] steps = new ProbabilityStep[]
    {
        new ProbabilityStep(0.1f, 0.4f),   // 10%偏离 → 40%概率
        new ProbabilityStep(0.2f, 0.6f),   // 20%偏离 → 60%概率
        new ProbabilityStep(0.4f, 0.8f),   // 40%偏离 → 80%概率
        new ProbabilityStep(0.8f, 0.95f),  // 80%偏离 → 95%概率
    };
}
/// <summary>
/// 市场事件系统 - 根据股价变化触发不同类型的事件卡牌
/// </summary>
public class MarketEventSystem : Singleton<MarketEventSystem>
{
    [Header("事件卡组配置")]
    [SerializeField] private List<CardData> bullCards = new();    // 做多卡组
    [SerializeField] private List<CardData> bearCards = new();    // 做空卡组
    [SerializeField] private List<CardData> neutralCards = new(); // 中立卡组

    [Header("触发时间配置")]
    [SerializeField] private float eventInterval = 10f;  // 事件触发间隔（秒）
    [SerializeField] private float baseEventChance = 0.7f; // 基础事件触发概率

    [Header("概率计算配置")]
    [SerializeField] private ProbabilityMode probabilityMode = ProbabilityMode.Exponential;
    [SerializeField] private float maxDeviationThreshold = 0.8f;    // 最大偏离阈值（80%）

    [Header("做多事件概率配置")]
    [SerializeField] private AnimationCurve bullProbabilityCurve;   // 做多概率曲线
    [SerializeField] private EventProbabilityConfig bullConfig = new EventProbabilityConfig(1.5f, 0.05f, 0.9f);
    [SerializeField] private ProbabilityStepConfig bullSteps = new ProbabilityStepConfig();

    [Header("做空事件概率配置")]
    [SerializeField] private AnimationCurve bearProbabilityCurve;   // 做空概率曲线
    [SerializeField] private EventProbabilityConfig bearConfig = new EventProbabilityConfig(1.5f, 0.05f, 0.9f);
    [SerializeField] private ProbabilityStepConfig bearSteps = new ProbabilityStepConfig();

    [Header("中立事件概率配置")]
    [SerializeField] private AnimationCurve neutralProbabilityCurve; // 中立概率曲线
    [SerializeField] private EventProbabilityConfig neutralConfig = new EventProbabilityConfig(0.5f, 0.2f, 0.5f);
    [SerializeField] private ProbabilityStepConfig neutralSteps = new ProbabilityStepConfig();

    // 卡牌索引列表（更高效的卡牌选择方式）
    private List<int> bullCardIndices = new();
    private List<int> bearCardIndices = new();
    private List<int> neutralCardIndices = new();

    // 计时器
    private Timer eventTimer;

    // 初始价格（用于计算偏离度）
    private float initialStockPrice;

    private void Start()
    {
        InitializeEventSystem();
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ChangeNPCStrategyGA>(ChangeNPCStrategyPerformer);
        ActionSystem.AttachPerformer<TriggerEventGA>(TriggerEventPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ChangeNPCStrategyGA>();
        ActionSystem.DetachPerformer<TriggerEventGA>();
        eventTimer?.StopTimer();
    }

    /// <summary>
    /// 初始化事件系统
    /// </summary>
    private void InitializeEventSystem()
    {
        // 记录初始股价
        initialStockPrice = StockSystem.Instance.initialStockPrice;

        // 初始化概率曲线（如果未设置）
        if (bullProbabilityCurve == null || bullProbabilityCurve.keys.Length == 0)
        {
            InitializeDefaultCurves();
        }

        // 初始化卡牌索引列表
        InitializeCardIndices();

        // 启动事件计时器
        StartEventTimer();

        Debug.Log($"市场事件系统已初始化，初始股价: {initialStockPrice:F2}");
    }

    /// <summary>
    /// 初始化卡牌索引列表
    /// </summary>
    private void InitializeCardIndices()
    {
        RefillCardIndices(EEventCardType.Bull);
        RefillCardIndices(EEventCardType.Bear);
        RefillCardIndices(EEventCardType.Neutral);

        Debug.Log($"卡牌索引已初始化: 做多({bullCardIndices.Count}) 做空({bearCardIndices.Count}) 中性({neutralCardIndices.Count})");
    }

    /// <summary>
    /// 重新填充指定类型的卡牌索引（预先洗牌）
    /// </summary>
    private void RefillCardIndices(EEventCardType type)
    {
        List<int> indices;
        int cardCount;

        switch (type)
        {
            case EEventCardType.Bull:
                indices = bullCardIndices;
                cardCount = bullCards.Count;
                break;
            case EEventCardType.Bear:
                indices = bearCardIndices;
                cardCount = bearCards.Count;
                break;
            case EEventCardType.Neutral:
                indices = neutralCardIndices;
                cardCount = neutralCards.Count;
                break;
            default:
                return;
        }

        indices.Clear();

        // 添加所有索引
        for (int i = 0; i < cardCount; i++)
        {
            indices.Add(i);
        }

        // 洗牌算法（Fisher-Yates shuffle）
        for (int i = indices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (indices[i], indices[randomIndex]) = (indices[randomIndex], indices[i]);
        }

        Debug.Log($"{type} 卡组索引已重置并洗牌，可用卡牌: {cardCount}");
    }

    /// <summary>
    /// 初始化默认概率曲线（基于初始价格偏离度）
    /// </summary>
    private void InitializeDefaultCurves()
    {
        // 做多概率曲线：价格越低于初始价格，做多概率越高
        bullProbabilityCurve = new AnimationCurve(
            new Keyframe(0f, 0.9f),      // 价格大幅低于初始价格时90%概率
            new Keyframe(0.5f, 0.3f),    // 价格等于初始价格时30%概率
            new Keyframe(1f, 0.05f)      // 价格大幅高于初始价格时5%概率
        );

        // 做空概率曲线：价格越高于初始价格，做空概率越高
        bearProbabilityCurve = new AnimationCurve(
            new Keyframe(0f, 0.05f),     // 价格大幅低于初始价格时5%概率
            new Keyframe(0.5f, 0.3f),    // 价格等于初始价格时30%概率
            new Keyframe(1f, 0.9f)       // 价格大幅高于初始价格时90%概率
        );
    }

    /// <summary>
    /// 启动事件计时器
    /// </summary>
    private void StartEventTimer()
    {
        if (eventTimer == null)
        {
            eventTimer = gameObject.AddComponent<Timer>();
        }

        eventTimer.SetDuration(eventInterval);
        eventTimer.SetCountdown(true);
        eventTimer.SetLoop(true);
        eventTimer.OnTimerComplete += TryTriggerEvent;
        eventTimer.StartTimer();

        Debug.Log($"事件计时器已启动，间隔: {eventInterval}秒");
    }

    /// <summary>
    /// 尝试触发事件
    /// </summary>
    private void TryTriggerEvent()
    {
        // 基础概率检查
        if (Random.value > baseEventChance)
        {
            Debug.Log("本次未触发事件");
            NewsSystem.Instance.BroadcastMarketEvent(EEventCardType.Neutral);
            return;
        }

        // 根据股价选择事件类型并触发
        EEventCardType selectedType = SelectEventTypeByStockPrice();
        CardData selectedCard = SelectCardFromType(selectedType);

        if (selectedCard != null)
        {
            TriggerEvent(selectedCard);
            NewsSystem.Instance.BroadcastMarketEvent(selectedType);
        }
        else
        {
            NewsSystem.Instance.BroadcastMarketEvent(EEventCardType.Neutral);
        }
    }

    /// <summary>
    /// 根据股价选择事件类型（支持多种概率模式）
    /// </summary>
    private EEventCardType SelectEventTypeByStockPrice()
    {
        float currentPrice = StockSystem.Instance.NextStockPrice;
        float priceDeviation = (currentPrice - initialStockPrice) / initialStockPrice;
        float absDeviation = Mathf.Abs(priceDeviation);

        // 根据模式计算所有事件概率
        float bullWeight, bearWeight, neutralWeight;
        CalculateEventProbabilities(priceDeviation, absDeviation, out bullWeight, out bearWeight);

        // 从计算结果中获取中立权重（临时处理，等待方法签名更新）
        switch (probabilityMode)
        {
            case ProbabilityMode.Exponential:
                CalculateExponentialProbabilities(priceDeviation, absDeviation, out _, out _, out neutralWeight);
                break;
            case ProbabilityMode.Stepped:
                CalculateSteppedProbabilities(priceDeviation, absDeviation, out _, out _, out neutralWeight);
                break;
            case ProbabilityMode.Curve:
                CalculateCurveProbabilities(priceDeviation, out _, out _, out neutralWeight);
                break;
            case ProbabilityMode.Linear:
            default:
                CalculateLinearProbabilities(priceDeviation, absDeviation, out _, out _, out neutralWeight);
                break;
        }

        // 随机选择
        float totalWeight = bullWeight + bearWeight + neutralWeight;
        float randomValue = Random.value * totalWeight;

        // 调试信息
        string modeInfo = GetModeDebugInfo(absDeviation, bullWeight, bearWeight, neutralWeight);
        float deviationPercent = priceDeviation * 100f;

        if (randomValue < bullWeight)
        {
            Debug.Log($"股价: {currentPrice:F2} (偏离: {deviationPercent:F1}%) {modeInfo}, 选择: 做多事件");
            return EEventCardType.Bull;
        }
        else if (randomValue < bullWeight + bearWeight)
        {
            Debug.Log($"股价: {currentPrice:F2} (偏离: {deviationPercent:F1}%) {modeInfo}, 选择: 做空事件");
            return EEventCardType.Bear;
        }
        else
        {
            Debug.Log($"股价: {currentPrice:F2} (偏离: {deviationPercent:F1}%) {modeInfo}, 选择: 中立事件");
            return EEventCardType.Neutral;
        }
    }

    /// <summary>
    /// 根据不同模式计算事件概率
    /// </summary>
    private void CalculateEventProbabilities(float priceDeviation, float absDeviation, out float bullWeight, out float bearWeight)
    {
        float neutralWeight;

        switch (probabilityMode)
        {
            case ProbabilityMode.Exponential:
                CalculateExponentialProbabilities(priceDeviation, absDeviation, out bullWeight, out bearWeight, out neutralWeight);
                break;
            case ProbabilityMode.Stepped:
                CalculateSteppedProbabilities(priceDeviation, absDeviation, out bullWeight, out bearWeight, out neutralWeight);
                break;
            case ProbabilityMode.Curve:
                CalculateCurveProbabilities(priceDeviation, out bullWeight, out bearWeight, out neutralWeight);
                break;
            case ProbabilityMode.Linear:
            default:
                CalculateLinearProbabilities(priceDeviation, absDeviation, out bullWeight, out bearWeight, out neutralWeight);
                break;
        }
    }

    /// <summary>
    /// 指数模式概率计算（分别计算三种事件）
    /// </summary>
    private void CalculateExponentialProbabilities(float priceDeviation, float absDeviation, out float bullWeight, out float bearWeight, out float neutralWeight)
    {
        // 分别计算每种事件的概率
        bullWeight = CalculateSingleEventProbability(priceDeviation, absDeviation, EEventCardType.Bull, bullConfig);
        bearWeight = CalculateSingleEventProbability(priceDeviation, absDeviation, EEventCardType.Bear, bearConfig);
        neutralWeight = CalculateSingleEventProbability(priceDeviation, absDeviation, EEventCardType.Neutral, neutralConfig);
    }

    /// <summary>
    /// 计算单个事件类型的指数概率
    /// </summary>
    private float CalculateSingleEventProbability(float priceDeviation, float absDeviation, EEventCardType eventType, EventProbabilityConfig config)
    {
        // 根据事件类型确定偏离方向的影响
        float effectiveDeviation = GetEffectiveDeviation(priceDeviation, absDeviation, eventType);

        // 指数函数计算
        float normalizedDeviation = effectiveDeviation / maxDeviationThreshold;
        float exponentialFactor = 1f - Mathf.Exp(-config.exponentialBase * normalizedDeviation);

        return Mathf.Lerp(config.minProbability, config.maxProbability, exponentialFactor);
    }

    /// <summary>
    /// 根据事件类型获取有效偏离度
    /// </summary>
    private float GetEffectiveDeviation(float priceDeviation, float absDeviation, EEventCardType eventType)
    {
        return eventType switch
        {
            EEventCardType.Bull => priceDeviation < 0 ? absDeviation : 0f,  // 做多：只在价格下跌时生效
            EEventCardType.Bear => priceDeviation > 0 ? absDeviation : 0f,  // 做空：只在价格上涨时生效
            EEventCardType.Neutral => maxDeviationThreshold - absDeviation, // 中立：偏离越小概率越高
            _ => 0f
        };
    }

    /// <summary>
    /// 阶梯模式概率计算
    /// </summary>
    private void CalculateSteppedProbabilities(float priceDeviation, float absDeviation, out float bullWeight, out float bearWeight, out float neutralWeight)
    {
        bullWeight = CalculateSteppedEventProbability(priceDeviation, absDeviation, EEventCardType.Bull, bullSteps);
        bearWeight = CalculateSteppedEventProbability(priceDeviation, absDeviation, EEventCardType.Bear, bearSteps);
        neutralWeight = CalculateSteppedEventProbability(priceDeviation, absDeviation, EEventCardType.Neutral, neutralSteps);
    }

    /// <summary>
    /// 计算单个事件类型的阶梯概率
    /// </summary>
    private float CalculateSteppedEventProbability(float priceDeviation, float absDeviation, EEventCardType eventType, ProbabilityStepConfig stepConfig)
    {
        float effectiveDeviation = GetEffectiveDeviation(priceDeviation, absDeviation, eventType);
        float eventProbability = GetConfigMinProbability(eventType);

        // 查找对应的阶梯
        foreach (var step in stepConfig.steps)
        {
            if (effectiveDeviation >= step.deviationThreshold)
            {
                eventProbability = step.probability;
            }
            else
            {
                break;
            }
        }

        return eventProbability;
    }

    /// <summary>
    /// 获取指定事件类型的最小概率
    /// </summary>
    private float GetConfigMinProbability(EEventCardType eventType)
    {
        return eventType switch
        {
            EEventCardType.Bull => bullConfig.minProbability,
            EEventCardType.Bear => bearConfig.minProbability,
            EEventCardType.Neutral => neutralConfig.minProbability,
            _ => 0.05f
        };
    }

    /// <summary>
    /// 曲线模式概率计算
    /// </summary>
    private void CalculateCurveProbabilities(float priceDeviation, out float bullWeight, out float bearWeight, out float neutralWeight)
    {
        float normalizedPrice = GetNormalizedStockPrice();
        bullWeight = bullProbabilityCurve?.Evaluate(normalizedPrice) ?? bullConfig.baseWeight;
        bearWeight = bearProbabilityCurve?.Evaluate(normalizedPrice) ?? bearConfig.baseWeight;
        neutralWeight = neutralProbabilityCurve?.Evaluate(normalizedPrice) ?? neutralConfig.baseWeight;
    }

    /// <summary>
    /// 线性模式概率计算
    /// </summary>
    private void CalculateLinearProbabilities(float priceDeviation, float absDeviation, out float bullWeight, out float bearWeight, out float neutralWeight)
    {
        bullWeight = CalculateLinearEventProbability(priceDeviation, absDeviation, EEventCardType.Bull, bullConfig);
        bearWeight = CalculateLinearEventProbability(priceDeviation, absDeviation, EEventCardType.Bear, bearConfig);
        neutralWeight = CalculateLinearEventProbability(priceDeviation, absDeviation, EEventCardType.Neutral, neutralConfig);
    }

    /// <summary>
    /// 计算单个事件类型的线性概率
    /// </summary>
    private float CalculateLinearEventProbability(float priceDeviation, float absDeviation, EEventCardType eventType, EventProbabilityConfig config)
    {
        float effectiveDeviation = GetEffectiveDeviation(priceDeviation, absDeviation, eventType);
        float normalizedDeviation = Mathf.Clamp01(effectiveDeviation / maxDeviationThreshold);

        return Mathf.Lerp(config.minProbability, config.maxProbability, normalizedDeviation);
    }

    /// <summary>
    /// 获取模式调试信息
    /// </summary>
    private string GetModeDebugInfo(float absDeviation, float bullWeight, float bearWeight, float neutralWeight)
    {
        return probabilityMode switch
        {
            ProbabilityMode.Exponential => $"[指数模式: 偏离{absDeviation:P1}, 做多{bullWeight:P1}, 做空{bearWeight:P1}, 中立{neutralWeight:P1}]",
            ProbabilityMode.Stepped => $"[阶梯模式: 偏离{absDeviation:P1}, 做多{bullWeight:P1}, 做空{bearWeight:P1}, 中立{neutralWeight:P1}]",
            ProbabilityMode.Curve => $"[曲线模式: 做多{bullWeight:P1}, 做空{bearWeight:P1}, 中立{neutralWeight:P1}]",
            ProbabilityMode.Linear => $"[线性模式: 偏离{absDeviation:P1}, 做多{bullWeight:P1}, 做空{bearWeight:P1}, 中立{neutralWeight:P1}]",
            _ => $"[未知模式]"
        };
    }

    /// <summary>
    /// 获取标准化股价（基于初始价格的偏离度，0-1范围）
    /// </summary>
    private float GetNormalizedStockPrice()
    {
        float currentPrice = StockSystem.Instance.NextStockPrice;

        // 计算相对于初始价格的偏离百分比
        float priceDeviation = (currentPrice - initialStockPrice) / initialStockPrice;

        // 将偏离度映射到0-1范围
        // -priceDeviationRange(比如-30%) 映射到 0
        // 0%(初始价格) 映射到 0.5
        // +priceDeviationRange(比如+30%) 映射到 1
        float normalizedValue = 0.5f + (priceDeviation / (maxDeviationThreshold * 2));

        // 确保在0-1范围内
        return Mathf.Clamp01(normalizedValue);
    }

    /// <summary>
    /// 从指定类型中选择未使用的卡牌（使用预洗牌的索引列表）
    /// </summary>
    private CardData SelectCardFromType(EEventCardType type)
    {
        List<int> indices;
        List<CardData> cards;

        switch (type)
        {
            case EEventCardType.Bull:
                indices = bullCardIndices;
                cards = bullCards;
                break;
            case EEventCardType.Bear:
                indices = bearCardIndices;
                cards = bearCards;
                break;
            case EEventCardType.Neutral:
                indices = neutralCardIndices;
                cards = neutralCards;
                break;
            default:
                return null;
        }

        // 如果索引列表为空，重新填充并洗牌
        if (indices.Count == 0)
        {
            RefillCardIndices(type);
        }

        // 再次检查是否有可用卡牌
        if (indices.Count == 0 || cards.Count == 0) return null;

        // 直接取最后一个索引（已经预先洗牌过了）
        int cardIndex = indices[indices.Count - 1];

        // 移除最后一个元素（O(1)操作）
        indices.RemoveAt(indices.Count - 1);

        // 返回对应的卡牌
        return cards[cardIndex];
    }

    /// <summary>
    /// 根据类型获取卡组
    /// </summary>
    private List<CardData> GetCardsByType(EEventCardType type)
    {
        return type switch
        {
            EEventCardType.Bull => bullCards,
            EEventCardType.Bear => bearCards,
            EEventCardType.Neutral => neutralCards,
            _ => new List<CardData>()
        };
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    private void TriggerEvent(CardData eventCard)
    {
        TriggerEventGA eventGA = new(eventCard);
        ActionSystem.Instance.Perform(eventGA);

        Debug.Log($"触发事件: {eventCard.Title} ");
    }
    private IEnumerator ChangeNPCStrategyPerformer(ChangeNPCStrategyGA changeNPCStrategyGA)
    {
        NPCSystem.Instance.NPCs.ForEach(npc =>
        {
            if (Random.value < changeNPCStrategyGA.ChangeProbability)
            {
                npc.ChangeStrategy(changeNPCStrategyGA.StrategyType);
            }
        });
        yield break;
    }

    /// <summary>
    /// 事件执行器
    /// </summary>
    private IEnumerator TriggerEventPerformer(TriggerEventGA eventGA)
    {
        Debug.Log($"执行事件: {eventGA.EventCard.Title}");

        // 将事件卡转换为Card并应用效果
        Card eventCard = new(eventGA.EventCard);

        foreach (AutoTargetEffect effect in eventCard.OtherEffects)
        {
            if (effect.Effect != null)
            {
                PerformEffectGA performEffectGA = new(effect.Effect, null);
                ActionSystem.Instance.Perform(performEffectGA);
            }
        }
        yield break;
    }

    #region 公共接口

    /// <summary>
    /// 手动触发指定类型的事件
    /// </summary>
    public void ManualTriggerEvent(EEventCardType type)
    {
        CardData selectedCard = SelectCardFromType(type);
        if (selectedCard != null)
        {
            TriggerEvent(selectedCard);
        }
    }

    /// <summary>
    /// 暂停事件系统
    /// </summary>
    public void PauseEventSystem()
    {
        eventTimer?.PauseTimer();
        Debug.Log("市场事件系统已暂停");
    }

    /// <summary>
    /// 恢复事件系统
    /// </summary>
    public void ResumeEventSystem()
    {
        eventTimer?.ResumeTimer();
        Debug.Log("市场事件系统已恢复");
    }

    /// <summary>
    /// 设置事件间隔
    /// </summary>
    public void SetEventInterval(float interval)
    {
        eventInterval = interval;
        if (eventTimer != null)
        {
            eventTimer.StopTimer();
            StartEventTimer();
        }
    }

    /// <summary>
    /// 重置所有卡组
    /// </summary>
    public void ResetAllCardPools()
    {
        RefillCardIndices(EEventCardType.Bull);
        RefillCardIndices(EEventCardType.Bear);
        RefillCardIndices(EEventCardType.Neutral);
        Debug.Log("所有事件卡组已重置");
    }

    #endregion
}