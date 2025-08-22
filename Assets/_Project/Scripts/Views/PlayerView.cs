using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class PlayerView : CharacterView
{
    [SerializeField] private MoneyDisplay moneyDisplay;

    [SerializeField] private List<StockDisplay> stockDisplays;

    [SerializeField] private ValuesDisplay valuesDisplay;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float doTweenUpdatePositionDuration = 0.15f;
    [SerializeField] private float cardPositionOffset = 0.01f;

    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;

    public Button nextRoundButton;

    private float doTweenScaleDuration;
    private float doTweenMoveDuration;

    private WaitForSeconds updateCardPositionTime;

    private List<CardView> cards = new();

    private void Awake()
    {
        updateCardPositionTime = new WaitForSeconds(doTweenUpdatePositionDuration);
    }
    private void Start()
    {
        nextRoundButton.onClick.AddListener(OnNextRoundButtonClick);
    }
    void OnDestroy()
    {
        nextRoundButton.onClick.RemoveListener(OnNextRoundButtonClick);
    }
    public void Setup(PlayerData playerData)
    {
        base.Setup(playerData);
        doTweenScaleDuration = playerData.doTweenScaleDuration;
        doTweenMoveDuration = playerData.doTweenMoveDuration;
    }


    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0) yield break;

        float cardSpacing = 1f / CurrentHandSize;
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing * 0.5f;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < cards.Count; i++)
        {
            // 检查卡牌是否仍然存在
            if (cards[i] == null || cards[i].gameObject == null)
            {
                continue;
            }

            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
            cards[i].transform.DOMove(splinePosition + transform.position + cardPositionOffset * i * Vector3.back, duration);
            cards[i].transform.DORotate(rotation.eulerAngles, duration);
        }

        yield return updateCardPositionTime;
    }

    private CardView GetCardView(Card card)
    {
        return cards.Where(cardView => cardView.Card == card).FirstOrDefault();
    }



    public override IEnumerator RemoveCardAnimation(Card card)
    {
        CardView cardView = GetCardView(card);

        if (cardView == null) yield break;

        cards.Remove(cardView);
        StartCoroutine(UpdateCardPositions(doTweenUpdatePositionDuration));
        cardView.transform.DOScale(Vector3.zero, doTweenScaleDuration);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, doTweenMoveDuration);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);

    }

    public override IEnumerator DrawCardAnimation(Card card)
    {
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        cards.Add(cardView);

        yield return StartCoroutine(UpdateCardPositions(doTweenUpdatePositionDuration));
    }
    public void OnNextRoundButtonClick()
    {
        NextRoundTurnGA nextRoundTurnGA = new();
        ActionSystem.Instance.Perform(nextRoundTurnGA);
    }
    public void UpdateMoneyText(float currentMoney)
    {
        moneyDisplay.UpdateMoney(currentMoney);
    }

    public void UpdateStockText(EStockType stockType, int currentStock)
    {
        foreach (var stockDisplay in stockDisplays)
        {
            if (stockDisplay.stockType == stockType)
            {
                stockDisplay.UpdateStock(currentStock);
            }
        }
    }

    public void UpdateAllDisplays()
    {
        valuesDisplay.UpdateValues();
        foreach (var card in cards)
        {
            card.Card.UpdateDescription();
            card.UpdateDescription();
        }
    }

    /// <summary>
    /// 清理所有卡牌和协程（用于重置）
    /// </summary>
    public void ClearAllCards()
    {
        // 停止所有协程
        StopAllCoroutines();

        // 销毁所有卡牌视图
        var cardsToDestroy = new List<CardView>(cards);
        foreach (var cardView in cardsToDestroy)
        {
            if (cardView != null)
            {
                Destroy(cardView.gameObject);
            }
        }

        // 清空卡牌列表
        cards.Clear();
    }

}