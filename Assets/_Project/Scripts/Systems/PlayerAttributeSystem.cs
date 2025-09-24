using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家属性系统
/// </summary>
public class PlayerAttributeSystem : SingletonCom<PlayerAttributeSystem>
{
    [SerializeField] public PlayerView playerView;
    [SerializeField] public PlayerAttributeDisplay playerAttributeDisplay;

    [Header("属性数据")]
    [SerializeField] private PlayerAttributesData playerAttributes;

    [Header("摸牌系统")]
    [SerializeField] private int baseCardsPerTurn = 5;
    public Dictionary<EStrategyType, float> ChangePricePersentDictionaryWhenBuy;
    public Dictionary<EStrategyType, float> ChangePriceDictionaryWhenBuy;
    public Dictionary<EStrategyType, float> ChangePricePersentDictionaryWhenSell;
    public Dictionary<EStrategyType, float> ChangePriceDictionaryWhenSell;

    protected override void Awake()
    {
        base.Awake();
        InitializeAttributes();
    }
    public void Setup(PlayerData playerData)
    {
        playerView.Setup(playerData);
        ChangePricePersentDictionaryWhenBuy = playerData.changePricePersentDictionaryWhenBuy.ToDictionary();
        ChangePriceDictionaryWhenBuy = playerData.changePriceDictionaryWhenBuy.ToDictionary();
        ChangePricePersentDictionaryWhenSell = playerData.changePricePersentDictionaryWhenSell.ToDictionary();
        ChangePriceDictionaryWhenSell = playerData.changePriceDictionaryWhenSell.ToDictionary();
        UpdateAllInfo();

    }

    private void OnEnable()
    {
        //丢弃卡牌
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        //改变属性
        ActionSystem.AttachPerformer<ChangeAttributeGA>(ChangeAttributePerformer);
        //监听 回合前后
        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);
        //改变金币
        ActionSystem.SubscribeReaction<ChangeMoneyGA>(ChangeMoneyPostReaction, ReactionTiming.POST);
        //改变股票数量
        ActionSystem.SubscribeReaction<ChangeStockGA>(ChangeStockPostReaction, ReactionTiming.POST);

        //监听属性变化
        ActionSystem.SubscribeReaction<ChangeAttributeGA>(ChangeAttributePostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<ChangeAttributeGA>();
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
    private IEnumerator ChangeAttributePerformer(ChangeAttributeGA action)
    {
        playerAttributes.GetAttribute(action.attributeType).currentValue += action.attributeValue;
        if (playerAttributes.GetAttribute(action.attributeType).currentValue < 0)
        {
            playerAttributes.GetAttribute(action.attributeType).currentValue = 0;
        }
        yield return null;
    }
    private void ChangeAttributePostReaction(ChangeAttributeGA action)
    {
        UpdateAllInfo();

    }

    private void ChangeMoneyPostReaction(ChangeMoneyGA action)
    {
        UpdateAllInfo();
    }

    private void ChangeStockPostReaction(ChangeStockGA action)
    {
        UpdateAllInfo();
    }

    #endregion

    #region Attribute Effects

    /// <summary>
    /// 获取属性值
    /// </summary>
    public float GetAttributeValue(EAttrType attributeType)
    {
        return playerAttributes.GetAttributeValue(attributeType);
    }

    /// <summary>
    /// 获取每回合摸牌数
    /// </summary>
    public int GetCardsPerTurn()
    {
        int socialBonus = (int)GetAttributeValue(EAttrType.Social);
        return baseCardsPerTurn + socialBonus;
    }
    /// <summary>
    /// 获取股市影响力加成
    /// </summary>
    public float GetStockInfluenceBonus()
    {
        float charisma = GetAttributeValue(EAttrType.Charisma);
        charisma = Mathf.Min(10, charisma);
        return 1 + charisma * 10f / 100f;
    }
    public float GetStockEnvironmentBonus()
    {
        float fanaticism = GetAttributeValue(EAttrType.Fanaticism);
        float calmness = GetAttributeValue(EAttrType.Calmness);
        float effect = fanaticism - calmness;
        effect = Mathf.Clamp(effect, -30, 30);
        float bonus = 1 + effect * 2f / 100f;
        return Mathf.Max(0.2f, bonus);
    }
    public float GetStockCourageBonus()
    {
        return 1 + GetAttributeValue(EAttrType.Courage) * 10f / 100f;
    }
    #endregion

    #region Reactions
    private void NextRoundTurnPreReaction(NextRoundTurnGA nextRoundTurnGA)
    {
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Ins.AddReaction(discardAllCardsGA);
    }

    private void NextRoundTurnPostReaction(NextRoundTurnGA nextRoundTurnGA)
    {
        DrawCardsGA drawCardsGA = new(GetCardsPerTurn(), playerView);
        ActionSystem.Ins.AddReaction(drawCardsGA);
        // 摸牌
        int cardsToDraw = GetCardsPerTurn();

        ChangeAttributeGA changeAttributeGA = new(EAttrType.Social, -1f);
        ActionSystem.Ins.AddReaction(changeAttributeGA);


        //刷新信息
        UpdateAllInfo();
    }
    #endregion

    public void UpdateAllInfo()
    {
        // 检查所有必要的组件是否仍然有效
        if (playerView == null || playerView.gameObject == null ||
            MultiStockSystem.Ins == null || this == null)
            return;

        playerView.UpdateMoneyText(MultiStockSystem.Ins.GetCurrentMoney());
        foreach (var stockType in Enum.GetValues(typeof(EStockType)))
        {
            playerView.UpdateStockText((EStockType)stockType, MultiStockSystem.Ins.GetStockHoldings((EStockType)stockType));
        }
        playerView.UpdateAllDisplays();

        if (playerAttributeDisplay != null && playerAttributeDisplay.gameObject != null)
        {
            playerAttributeDisplay.UpdateAllDisplays();
        }
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
    /// 重置玩家属性系统到初始状态
    /// </summary>
    public void ResetSystem()
    {
        // 重置所有属性值为0
        foreach (var attribute in playerAttributes.attributes)
        {
            attribute.currentValue = 0f;
        }

        // 重置统计信息
        playerAttributes.totalAttributePoints = 0;
        playerAttributes.totalMoneySpent = 0;

        // 清空玩家手牌和牌堆，停止协程
        if (playerView != null)
        {
            // 使用PlayerView的清理方法
            playerView.ClearAllCards();

            // 清空数据模型
            playerView.hand.Clear();
            playerView.drawPile.Clear();
            playerView.DiscardPile.Clear();
        }

        // 更新UI显示
        UpdateAllInfo();
    }
    #endregion
}
