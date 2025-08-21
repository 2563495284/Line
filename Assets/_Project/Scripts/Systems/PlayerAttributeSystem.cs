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

    [Header("摸牌系统")]
    [SerializeField] private int baseCardsPerTurn = 5;

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
        // 摸牌
        int cardsToDraw = GetCardsPerTurn();

        ChangeAttributeGA changeAttributeGA = new(EPlayerAttributeType.Social, -1f);
        ActionSystem.Instance.AddReaction(changeAttributeGA);

        ChangeAttributeGA changeAttributeGA2 = new(EPlayerAttributeType.Patience, -1f);
        ActionSystem.Instance.AddReaction(changeAttributeGA2);
        //刷新信息
        UpdateAllInfo();
    }
    #endregion

    public void UpdateAllInfo()
    {
        playerView.UpdateMoneyText(MultiStockSystem.Instance.GetCurrentMoney());
        foreach (var stockType in Enum.GetValues(typeof(EStockType)))
        {
            playerView.UpdateStockText((EStockType)stockType, MultiStockSystem.Instance.GetStockHoldings((EStockType)stockType));
        }
        playerView.UpdateAllDisplays();
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
    #endregion
}
