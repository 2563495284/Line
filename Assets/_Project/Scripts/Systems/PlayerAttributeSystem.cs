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
    [SerializeField] public PlayerAttributeDisplay playerAttributeDisplay;

    [Header("属性数据")]
    [SerializeField] private PlayerAttributesData playerAttributes;

    [Header("能量系统")]
    [SerializeField] private int baseEnergyPerTurn = 3;
    [SerializeField] private int currentMana = 3;
    [SerializeField] private int maxMana = 10;
    [SerializeField] private int savedMana = 0; // 耐心属性保存的能量

    [Header("摸牌系统")]
    [SerializeField] private int baseCardsPerTurn = 5;

    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;

    protected override void Awake()
    {
        base.Awake();
        InitializeAttributes();
    }
    public void Setup(PlayerData playerData)
    {
        playerView.Setup(playerData);
        UpdateAllInfo();
    }

    private void OnEnable()
    {
        //改变能量
        ActionSystem.AttachPerformer<ChangeManaGA>(ChangeManaPerformer);
        //存储能量
        ActionSystem.AttachPerformer<RestoreEnergyGA>(RestoreEnergyPerformer);
        //丢弃卡牌
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        //监听 回合前后
        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);
        //改变金币
        ActionSystem.SubscribeReaction<ChangeMoneyGA>(ChangeMoneyPostReaction, ReactionTiming.POST);
        //改变股票数量
        ActionSystem.SubscribeReaction<ChangeStockGA>(ChangeStockPostReaction, ReactionTiming.POST);
        //改变属性
        ActionSystem.SubscribeReaction<ChangeAttributeGA>(ChangeAttributePostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ChangeManaGA>();
        ActionSystem.DetachPerformer<RestoreEnergyGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<ChangeMoneyGA>(ChangeMoneyPostReaction, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<ChangeStockGA>(ChangeStockPostReaction, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<ChangeAttributeGA>(ChangeAttributePostReaction, ReactionTiming.POST);
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
        currentMana = GetTotalEnergyPerTurn();
    }

    #endregion

    #region GameAction Performers
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
        if (currentMana >= action.Amount)
        {
            currentMana -= action.Amount;

            if (showDebugInfo)
            {
                Debug.Log($"使用能量: {action.Amount}，剩余: {currentMana}");
            }
        }
        else
        {
            Debug.LogWarning($"能量不足，需要 {action.Amount}，拥有 {currentMana}");
        }

        yield return null;
    }

    /// <summary>
    /// 处理能量恢复
    /// </summary>
    private IEnumerator RestoreEnergyPerformer(RestoreEnergyGA action)
    {
        int oldEnergy = currentMana;
        currentMana = Mathf.Min(currentMana + action.Amount, maxMana);

        if (showDebugInfo)
        {
            Debug.Log($"恢复能量: {action.Amount}，{oldEnergy} -> {currentMana}");
        }

        yield return null;
    }

    private void ChangeMoneyPostReaction(ChangeMoneyGA action)
    {
        UpdateAllInfo();
    }

    private void ChangeStockPostReaction(ChangeStockGA action)
    {
        UpdateAllInfo();
    }

    private void ChangeAttributePostReaction(ChangeAttributeGA action)
    {
        playerAttributes.GetAttribute(action.attributeType).currentValue += action.attributeValue;
        UpdateAllInfo();
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
        return GetEnergyPerTurn() + savedMana;
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
        return 1 + GetAttributeValue(EPlayerAttributeType.Charisma) * 10f / 100f;
    }
    public float GetStockEnvironmentBonus()
    {
        return 1 + (GetAttributeValue(EPlayerAttributeType.Fanaticism) - GetAttributeValue(EPlayerAttributeType.Calmness)) * 10f / 100f;
    }
    public float GetStockCourageBonus()
    {
        return 1 + GetAttributeValue(EPlayerAttributeType.Courage) * 10f / 100f;
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
        DrawCardsGA drawCardsGA = new(GetCardsPerTurn(), playerView);
        ActionSystem.Instance.AddReaction(drawCardsGA);


        // 保存剩余能量（耐心属性）
        int saveableAmount = GetSaveableEnergy();
        int energyToSave = Mathf.Min(currentMana, saveableAmount);
        savedMana = energyToSave;

        // 恢复能量
        int energyToRestore = GetEnergyPerTurn();
        currentMana = Mathf.Min(energyToRestore + savedMana, maxMana);

        // 摸牌
        int cardsToDraw = GetCardsPerTurn();

        // 触发事件
        ActionSystem.Instance.AddReaction(new ChangeManaGA(currentMana));

        //刷新信息
        UpdateAllInfo();
        if (showDebugInfo)
        {
            Debug.Log($"新回合开始 - 能量: {currentMana} (保存: {savedMana}), 摸牌: {cardsToDraw}");
        }
    }
    #endregion

    public void UpdateAllInfo()
    {
        playerView.UpdateMoneyText(MultiStockSystem.Instance.GetCurrentMoney());
        foreach (var stockType in Enum.GetValues(typeof(EStockType)))
        {
            playerView.UpdateStockText((EStockType)stockType, MultiStockSystem.Instance.GetStockHoldings((EStockType)stockType));
        }
        playerView.UpdateAllValuesText();
        playerAttributeDisplay.UpdateAllDisplays();
    }
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
        return currentMana;
    }

    /// <summary>
    /// 检查是否有足够能量
    /// </summary>
    public bool HasEnoughMana(int amount)
    {
        return currentMana >= amount;
    }
    #endregion
}
