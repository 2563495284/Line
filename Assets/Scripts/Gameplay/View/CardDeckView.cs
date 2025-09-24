using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;
public interface ICardEffectTarget
{
    void PreviewEffect();
    void CancelPreviewEffect();
    IEffectReceiver GetReceiver();
}
public class CardDeckView : LevelView
{
    private List<CardCom> cards = new();
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private CardCom coverCard;
    [SerializeField] private Transform cardFolder;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private TargetSelectView arrowView;
    /// <summary>
    /// 牌组整理时间
    /// </summary>
    [SerializeField] private float arrangeTime = 0.15f;
    /// <summary>
    /// 单个卡牌的动效时间
    /// </summary>
    [SerializeField] private float tweenTime = 0.15f;

    [SerializeField] private float cardPositionOffset = 0.01f;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;

    private Coroutine arrangeCor = null;
    public CardCom GetCard(CardModel data)
    {
        CardCom newCard = cardPrefab.OPGet(cardFolder).GetComponent<CardCom>();
        newCard.Init(data.cardId);
        newCard.inputAction += CardInputHandler;
        return newCard;
    }
    public void RecycleCard(CardCom com)
    {
        com.inputAction -= CardInputHandler;
        com.gameObject.OPPush();
    }
    private void CardInputHandler(CardComEventArgs args)
    {
        if (GM.Ins.Level.IsInPerform) return;
        switch (args.inputType)
        {
            case CardInputType.MouseDown:
                AddCMD(new SelectCardCMD(args.target.cardData, MUtils.GetMouseWp()));
                break;
            case CardInputType.MouseDrag:
                AddCMD(new DragCardCMD(args.target.cardData, MUtils.GetMouseWp()));
                break;
            case CardInputType.MouseUp:
                AddCMD(new ReleaseCardCMD(args.target.cardData));
                break;
            case CardInputType.MouseEnter:
                AddCMD(new PreviewCardCMD(args.target.cardData));
                break;
            case CardInputType.MouseExit:
                AddCMD(new CancelPreviewCardCMD(args.target.cardData));
                break;
        }
    }
    protected override void OnShow()
    {
        Register(NotifyConst.UpdatePlayerAttr, OnUpdatePlayerAttr);
        Register<PreviewCardArgs>(NotifyConst.CancelPreviewCard, OnCancelPreviewCard);
        Register<PreviewCardArgs>(NotifyConst.PreviewCard, OnPreviewCard);
        Register<CardTargetArgs>(NotifyConst.SelectCardTarget, OnSelectCardTarget);
        Register<CardTargetArgs>(NotifyConst.CancelSelectCardTarget, OnCancelSelectCardTarget);
        Register<DragCardArgs>(NotifyConst.StartDragCard, OnStartDragCard);
        Register<DragCardArgs>(NotifyConst.CancelSelectCard, OnCancelSelectCard);
        Register<DragCardArgs>(NotifyConst.DraggingCard, OnDraggingCard);
        Bind<PlayCardArgs>(PerformRequest.Play_DiscardCard, DiscardCard);
        Bind<PlayCardArgs>(PerformRequest.Play_EffectCard, EffectCard);
        Bind<PlayCardArgs>(PerformRequest.Play_PresentCard, PresentCard);
        Bind(PerformRequest.DiscardCardAll, DiscardCards);
        Bind<DeckOPArgs>(PerformRequest.DrawCards, DrawCards);
        Bind(PerformRequest.RefillCards, RefillDeck);

    }
    protected override void OnHide()
    {
        Unregister(NotifyConst.UpdatePlayerAttr, OnUpdatePlayerAttr);
        Unregister<PreviewCardArgs>(NotifyConst.CancelPreviewCard, OnCancelPreviewCard);
        Unregister<PreviewCardArgs>(NotifyConst.PreviewCard, OnPreviewCard);
        Unregister<CardTargetArgs>(NotifyConst.SelectCardTarget, OnSelectCardTarget);
        Unregister<CardTargetArgs>(NotifyConst.CancelSelectCardTarget, OnCancelSelectCardTarget);
        Unregister<DragCardArgs>(NotifyConst.StartDragCard, OnStartDragCard);
        Unregister<DragCardArgs>(NotifyConst.CancelSelectCard, OnCancelSelectCard);
        Unregister<DragCardArgs>(NotifyConst.DraggingCard, OnDraggingCard);
        Unbind<PlayCardArgs>(PerformRequest.Play_DiscardCard, DiscardCard);
        Unbind<PlayCardArgs>(PerformRequest.Play_EffectCard, EffectCard);
        Unbind<PlayCardArgs>(PerformRequest.Play_PresentCard, PresentCard);
        Unbind(PerformRequest.DiscardCardAll, DiscardCards);
        Unbind<DeckOPArgs>(PerformRequest.DrawCards, DrawCards);
        Unbind(PerformRequest.RefillCards, RefillDeck);
        cardPrefab.OPClear();
        cards.Clear();
    }

    private void OnDraggingCard(DragCardArgs args)
    {
        if (args.card.Cfg.releaseType == ECardReleaseType.NoTarget)
        {
            GetCard(args.card).transform.position = args.worldPos;
        }
        else if (args.card.Cfg.releaseType == ECardReleaseType.TargetToStock)
        {
            arrowView.SetWorldPos(args.worldPos);
        }
    }

    private void OnCancelSelectCard(DragCardArgs args)
    {
        ArrangeCards();
        arrowView.Hide();
        GetCard(args.card).ShowWrapper();
    }

    private void OnStartDragCard(DragCardArgs args)
    {
        CardCom card = GetCard(args.card);
        if (args.card.Cfg.releaseType == ECardReleaseType.TargetToStock)
        {
            arrowView.Show(args.worldPos);
        }
        else
        {
            card.ShowWrapper();
        }
        card.transform.SetPositionAndRotation(args.worldPos, Quaternion.identity);
    }

    private void OnCancelSelectCardTarget(CardTargetArgs args)
    {

    }

    private void OnSelectCardTarget(CardTargetArgs args)
    {

    }

    private void OnPreviewCard(PreviewCardArgs args)
    {
        coverCard.Init(args.card.cardId);
        coverCard.gameObject.SetActive(true);
        CardCom com = GetCard(args.card);
        com.HideWrapper();
        coverCard.transform.position = new(com.transform.position.x, -2, 0);
    }

    private void OnCancelPreviewCard(PreviewCardArgs args)
    {
        CardCom com = GetCard(args.card);
        com.HideWrapper();
        coverCard.gameObject.SetActive(false);
    }

    private void OnUpdatePlayerAttr()
    {
        cards.ForEach(e => e.UpdateDesc());
    }
    private Coroutine ArrangeCards()
    {
        if (arrangeCor != null)
            StopCoroutine(arrangeCor);
        arrangeCor = StartCoroutine(UpdateCardPositions(arrangeTime));
        return arrangeCor;
    }
    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0) yield break;

        float cardSpacing = 1f / cards.Count;
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing * 0.5f;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < cards.Count; i++)
        {
            // 检查卡牌是否仍然存在
            if (cards[i] == null || cards[i].gameObject == null)
                continue;

            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
            cards[i].transform.DOMove(splinePosition + transform.position + cardPositionOffset * i * Vector3.back, duration);
            cards[i].transform.DORotate(rotation.eulerAngles, duration);
        }
        yield return new WaitForSeconds(duration);
        arrangeCor = null;
    }
    /// <summary>
    /// 出示卡牌
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private IEnumerator PresentCard(PlayCardArgs args)
    {
        yield return null;
    }
    private IEnumerator RefillDeck()
    {
        yield return null;
    }
    /// <summary>
    /// 发动卡牌效果
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private IEnumerator EffectCard(PlayCardArgs args)
    {
        yield return null;
    }
    private IEnumerator DiscardCard(PlayCardArgs args)
    {
        CardCom cardView = GetCard(args.card);
        cards.Remove(cardView);
        ArrangeCards();
        cardView.transform.DOScale(Vector3.zero, tweenTime);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, tweenTime);
        yield return tween.WaitForCompletion();
        cardView.gameObject.OPPush();
    }
    private IEnumerator DiscardCards()
    {
        foreach (CardCom cardView in cards)
        {
            cards.Remove(cardView);
            cardView.transform.DOScale(Vector3.zero, tweenTime);
            cardView.transform.DOMove(discardPilePoint.position, tweenTime);
            cardView.gameObject.OPPush();
        }
        ArrangeCards();
        yield return new WaitForSeconds(tweenTime);
    }
    private IEnumerator DrawCards(DeckOPArgs args)
    {
        for (int i = 0; i < args.cards.Count; i++)
        {
            yield return DrawCardAnimation(args.cards[i]);
        }
        yield return null;
    }

    private IEnumerator DrawCardAnimation(CardModel card)
    {
        CardCom cardView = cardPrefab.OPGet(cardFolder).GetComponent<CardCom>();
        cards.Add(cardView);
        cardView.Init(card.cardId);
        if (arrangeCor != null)
            StopCoroutine(arrangeCor);
        arrangeCor = StartCoroutine(UpdateCardPositions(arrangeTime));
        yield return arrangeCor;
    }
}