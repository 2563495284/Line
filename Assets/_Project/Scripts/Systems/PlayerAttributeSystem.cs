using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家属性系统
/// </summary>
public class PlayerAttributeSystem : Singleton<PlayerAttributeSystem>
{
    [SerializeField] public PlayerView playerView;

    [Header("属性数据")]
    [SerializeField] private PlayerAttributesData playerAttributes;

    [Header("能量系统")]
    [SerializeField] private int baseEnergyPerTurn = 3;
    [SerializeField] private int currentEnergy = 3;
    [SerializeField] private int maxEnergy = 10;
    [SerializeField] private int savedEnergy = 0; // 耐心属性保存的能量

    [Header("摸牌系统")]
    [SerializeField] private int baseCardsPerTurn = 5;

    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;

    // 事件
    public event Action<EPlayerAttributeType, int> OnAttributeUpgraded;

    protected override void Awake()
    {
        base.Awake();
        InitializeAttributes();
    }
    public void Setup(PlayerData playerData)
    {
        playerView.Setup(playerData);
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<UpgradeAttributeGA>(UpgradeAttributePerformer);
        ActionSystem.AttachPerformer<ChangeManaGA>(ChangeManaPerformer);
        ActionSystem.AttachPerformer<RestoreEnergyGA>(RestoreEnergyPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<UpgradeAttributeGA>();
        ActionSystem.DetachPerformer<ChangeManaGA>();
        ActionSystem.DetachPerformer<RestoreEnergyGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);
    }

    #region Initialization

    /// <summary>
    /// 初始化属性系统
    /// </summary>
    private void InitializeAttributes()
    {
        if (playerAttributes == null)
        {
            playerAttributes = new PlayerAttributesData();
        }

        // 初始化能量
        currentEnergy = GetTotalEnergyPerTurn();
    }

    #endregion

    #region GameAction Performers

    /// <summary>
    /// 处理属性升级
    /// </summary>
    private IEnumerator UpgradeAttributePerformer(UpgradeAttributeGA action)
    {
        if (MultiStockSystem.Instance == null)
        {
            Debug.LogError("MultiStockSystem未找到，无法升级属性");
            yield break;
        }

        float availableMoney = MultiStockSystem.Instance.GetCurrentMoney();
        var attribute = playerAttributes.GetAttribute(action.AttributeType);

        if (attribute == null)
        {
            Debug.LogError($"未找到属性类型: {action.AttributeType}");
            yield break;
        }

        int upgradeCost = attribute.GetNextUpgradeCost();
        if (upgradeCost < 0)
        {
            Debug.LogWarning($"{attribute.attributeName} 已达到最大等级");
            yield break;
        }

        if (upgradeCost > availableMoney)
        {
            Debug.LogWarning($"金币不足，需要 {upgradeCost}，拥有 {availableMoney:F0}");
            yield break;
        }

        // 扣除金币
        var changeMoneyGA = new ChangeMoneyGA(-upgradeCost);
        ActionSystem.Instance.Perform(changeMoneyGA);

        // 升级属性
        int oldLevel = attribute.currentLevel;
        attribute.UpgradeAttribute((int)availableMoney);

        // 触发事件
        OnAttributeUpgraded?.Invoke(action.AttributeType, attribute.currentLevel);

        if (showDebugInfo)
        {
            Debug.Log($"{attribute.attributeName} 升级: Lv.{oldLevel} -> Lv.{attribute.currentLevel} " +
                     $"({attribute.currentValue}) 花费: {upgradeCost}金币");
        }

        yield return null;
    }
    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {
        CharacterView characterView = playerView;
        foreach (Card card in characterView.hand)
        {
            yield return characterView.RemoveCard(card);

        }

        characterView.hand.Clear();
    }

    /// <summary>
    /// 处理能量使用
    /// </summary>
    private IEnumerator ChangeManaPerformer(ChangeManaGA action)
    {
        if (currentEnergy >= action.Amount)
        {
            currentEnergy -= action.Amount;

            if (showDebugInfo)
            {
                Debug.Log($"使用能量: {action.Amount}，剩余: {currentEnergy}");
            }
        }
        else
        {
            Debug.LogWarning($"能量不足，需要 {action.Amount}，拥有 {currentEnergy}");
        }

        yield return null;
    }

    /// <summary>
    /// 处理能量恢复
    /// </summary>
    private IEnumerator RestoreEnergyPerformer(RestoreEnergyGA action)
    {
        int oldEnergy = currentEnergy;
        currentEnergy = Mathf.Min(currentEnergy + action.Amount, maxEnergy);

        if (showDebugInfo)
        {
            Debug.Log($"恢复能量: {action.Amount}，{oldEnergy} -> {currentEnergy}");
        }

        yield return null;
    }

    #endregion

    #region Attribute Effects

    /// <summary>
    /// 获取属性值
    /// </summary>
    public float GetAttributeValue(EPlayerAttributeType attributeType)
    {
        return playerAttributes.GetAttributeValue(attributeType);
    }

    /// <summary>
    /// 获取每回合摸牌数
    /// </summary>
    public int GetCardsPerTurn()
    {
        int socialBonus = (int)GetAttributeValue(EPlayerAttributeType.Social);
        return baseCardsPerTurn + socialBonus;
    }

    /// <summary>
    /// 获取每回合能量恢复数
    /// </summary>
    public int GetEnergyPerTurn()
    {
        int wisdomBonus = (int)GetAttributeValue(EPlayerAttributeType.Wisdom);
        return baseEnergyPerTurn + wisdomBonus;
    }

    /// <summary>
    /// 获取总能量上限（包括保存的能量）
    /// </summary>
    public int GetTotalEnergyPerTurn()
    {
        return GetEnergyPerTurn() + savedEnergy;
    }

    /// <summary>
    /// 获取可保存的能量数量
    /// </summary>
    public int GetSaveableEnergy()
    {
        return (int)GetAttributeValue(EPlayerAttributeType.Patience);
    }

    /// <summary>
    /// 获取股市影响力加成
    /// </summary>
    public float GetStockInfluenceBonus()
    {
        return GetAttributeValue(EPlayerAttributeType.Charisma) / 100f;
    }

    #endregion

    #region Reactions
    private void NextRoundTurnPreReaction(NextRoundTurnGA nextRoundTurnGA)
    {
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }

    private void NextRoundTurnPostReaction(NextRoundTurnGA nextRoundTurnGA)
    {
        DrawCardsGA drawCardsGA = new(playerView.MaxHandSize, playerView);
        ActionSystem.Instance.AddReaction(drawCardsGA);


        // 保存剩余能量（耐心属性）
        int saveableAmount = GetSaveableEnergy();
        int energyToSave = Mathf.Min(currentEnergy, saveableAmount);
        savedEnergy = energyToSave;

        // 恢复能量
        int energyToRestore = GetEnergyPerTurn();
        currentEnergy = Mathf.Min(energyToRestore + savedEnergy, maxEnergy);

        // 摸牌
        int cardsToDraw = GetCardsPerTurn();

        // 触发事件
        ActionSystem.Instance.AddReaction(new ChangeManaGA(currentEnergy));

        if (showDebugInfo)
        {
            Debug.Log($"新回合开始 - 能量: {currentEnergy} (保存: {savedEnergy}), 摸牌: {cardsToDraw}");
        }
    }
    #endregion


    #region Public Interface

    /// <summary>
    /// 获取所有属性数据
    /// </summary>
    public PlayerAttributesData GetPlayerAttributes()
    {
        return playerAttributes;
    }

    /// <summary>
    /// 获取当前能量
    /// </summary>
    public int GetCurrentEnergy()
    {
        return currentEnergy;
    }

    /// <summary>
    /// 检查是否有足够能量
    /// </summary>
    public bool HasEnoughEnergy(int amount)
    {
        return currentEnergy >= amount;
    }

    /// <summary>
    /// 获取属性升级费用
    /// </summary>
    public int GetUpgradeCost(EPlayerAttributeType attributeType)
    {
        var attribute = playerAttributes.GetAttribute(attributeType);
        return attribute?.GetNextUpgradeCost() ?? -1;
    }

    /// <summary>
    /// 检查是否可以升级属性
    /// </summary>
    public bool CanUpgradeAttribute(EPlayerAttributeType attributeType)
    {
        var attribute = playerAttributes.GetAttribute(attributeType);
        if (attribute == null || attribute.currentLevel >= attribute.maxLevel)
            return false;

        if (MultiStockSystem.Instance == null)
            return false;

        int cost = attribute.GetNextUpgradeCost();
        float availableMoney = MultiStockSystem.Instance.GetCurrentMoney();
        return cost <= availableMoney;
    }

    #endregion
}
