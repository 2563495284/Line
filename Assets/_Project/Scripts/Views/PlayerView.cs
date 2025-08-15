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
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float doTweenUpdatePositionDuration = 0.15f;
    [SerializeField] private float cardPositionOffset = 0.01f;

    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;

    public Button drawCardButton;

    public float drawCardCostMoney = -100f;
    public int drawCardCount = 2;

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
        drawCardButton.onClick.AddListener(OnDrawCardButtonClick);
    }
    void OnDestroy()
    {
        drawCardButton.onClick.RemoveListener(OnDrawCardButtonClick);
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

    public void OnDrawCardButtonClick()
    {
        if (StockSystem.Instance.CurrentMoney < drawCardCostMoney)
        {
            TipsSystem.Instance.ShowError("资金不足！");
            return;
        }
        DrawCardsGA drawCardsGA = new(drawCardCount, this);
        ActionSystem.Instance.Perform(drawCardsGA);
        ChangeMoneyGA changeMoneyGA = new(drawCardCostMoney);
        ActionSystem.Instance.Perform(changeMoneyGA);
    }

}