using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
enum EViewPhaseState
{
    None,
    Action,
    Trade
}
public class CardView : LevelView
{
    [Header("配置")]
    public float arrangeTime = 0.2f;
    public float intervalTime = 0.1f;
    [Header("引用")]
    public SpriteButton passButton;
    private DList<int, CardCom> cards = new();
    public Transform drawFromPoint;
    public Transform discardToPoint;
    public Transform effectPoint;
    public Transform validHeightPoint;
    public Transform handFolder;
    public Spline cardsSpline;
    private GameObject CardPrefab => LoadManager.Ins.GetRes<GameObject>(ResPath.Level.Key, ResPath.Level.CardCom);
    private bool isCardsActive = false;
    private EViewPhaseState phaseState = EViewPhaseState.None;
    protected override void OnAwake()
    {
        base.OnAwake();
        passButton.onClick.AddListener(OnClickPassRound);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Register(EventConst.EnterLevelView, HideView);
        Register(EventConst.EnterRouteView, HideView);
        Register(EventConst.EnterShopView, HideView);
        Register(EventConst.EnterRoundView, ShowView);
        Bind<RefillCardsArgs>(EventConst.RefillCards, RefillCardsPerformer);
        Bind<DrawActionCardsArgs>(EventConst.DrawActionCards, DrawActionCardsPerformer);
        Bind<DiscardActionCardsArgs>(EventConst.DiscardActionCards, DiscardActionCardsPerformer);
        Bind<CardEffectStartArgs>(EventConst.CardEffectStart, CardEffectStartPerformer);
        Bind<DrawTradeCardsArgs>(EventConst.DrawTradeCards, DrawTradeCardsPerformer);
        Bind<DiscardTradeCardsArgs>(EventConst.DiscardTradeCards, DiscardTradeCardsPerformer);
        Bind(EventConst.EnterActionPhase, EnterActionPhasePerformer);
        Bind(EventConst.ExitActionPhase, ExitActionPhasePerformer);
        Bind(EventConst.EnterTradePhase, EnterTradePhasePerformer);
        Bind(EventConst.ExitTradePhase, ExitTradePhasePerformer);
        Register<DrawActionCardsCMD>(ActiveCards, ReactionTiming.POST);
        Register<ReleaseCardCMD>(BanCards, ReactionTiming.PRE);
        Register<ReleaseCardCMD>(ActiveCards, ReactionTiming.POST);
    }
    public override void OnExit()
    {
        base.OnExit();
        Unregister(EventConst.EnterLevelView, HideView);
        Unregister(EventConst.EnterRouteView, HideView);
        Unregister(EventConst.EnterShopView, HideView);
        Unregister(EventConst.EnterRoundView, ShowView);
        Unbind<RefillCardsArgs>(EventConst.RefillCards, RefillCardsPerformer);
        Unbind<DrawActionCardsArgs>(EventConst.DrawActionCards, DrawActionCardsPerformer);
        Unbind<DiscardActionCardsArgs>(EventConst.DiscardActionCards, DiscardActionCardsPerformer);
        Unbind<CardEffectStartArgs>(EventConst.CardEffectStart, CardEffectStartPerformer);
        Unbind<DrawTradeCardsArgs>(EventConst.DrawTradeCards, DrawTradeCardsPerformer);
        Unbind<DiscardTradeCardsArgs>(EventConst.DiscardTradeCards, DiscardTradeCardsPerformer);
        Unbind(EventConst.EnterActionPhase, EnterActionPhasePerformer);
        Unbind(EventConst.ExitActionPhase, ExitActionPhasePerformer);
        Unbind(EventConst.EnterTradePhase, EnterTradePhasePerformer);
        Unbind(EventConst.ExitTradePhase, ExitTradePhasePerformer);
        Unregister<DrawActionCardsCMD>();
        Unregister<ReleaseCardCMD>();
    }
    private void ActiveCards()
    {
        isCardsActive = true;
    }
    private void BanCards()
    {
        isCardsActive = false;
    }
    private void HideView()
    {
        gameObject.SetActive(false);
    }

    private void ShowView()
    {
        gameObject.SetActive(true);
    }
    private void OnClickPassRound()
    {
        if (phaseState == EViewPhaseState.Action)
            ExeCMD(new FinishActionCMD());
        else if (phaseState == EViewPhaseState.Trade)
            ExeCMD(new FinishTradeCMD());
    }
    private CardCom NewCard(int cardId)
    {
        GameObject cardGO = CardPrefab.OPGet(handFolder);
        CardCom card = cardGO.GetComponent<CardCom>();
        card.SetData(cardId);
        card.Reset();
        card.transform.SetPositionAndRotation(handFolder.transform.position, Quaternion.identity);
        card.On(CardCom.ExitEvt, OnExitCard);
        card.On(CardCom.PreviewEvt, OnPreviewCard);
        card.On(CardCom.SelectEvt, OnSelectCard);
        card.On(CardCom.ReleaseEvt, OnReleaseCard);
        return card;
    }
    private void RecycleCard(CardCom card)
    {
        card.Off(CardCom.ExitEvt, OnExitCard);
        card.Off(CardCom.PreviewEvt, OnPreviewCard);
        card.Off(CardCom.SelectEvt, OnSelectCard);
        card.Off(CardCom.ReleaseEvt, OnReleaseCard);
        card.BanHandle();
        card.gameObject.OPPush();
    }
    private CardCom selectedCard = null;
    private void OnExitCard(object cardObj)
    {
        if (selectedCard != null || !isCardsActive)
            return;
        CardCom card = cardObj as CardCom;
        card.CancelPreview();
    }
    private void OnPreviewCard(object cardObj)
    {
        if (selectedCard != null || !isCardsActive)
            return;
        CardCom card = cardObj as CardCom;
        foreach (var e in cards)
        {
            e.CancelPreview();
            e.SetOrder(cards.IndexOf(e));
        }
        card.Preview();
        card.SetOrder(50);
    }
    private void OnSelectCard(object cardObj)
    {
        if (selectedCard != null || !isCardsActive)
            return;
        CardCom card = cardObj as CardCom;
        selectedCard = card;
        card.SetCancel();
        isValidRelease = false;
        card.CancelPreview();
        card.transform.rotation = Quaternion.identity;

    }
    private void OnReleaseCard(object cardObj)
    {
        if (selectedCard == null || !isCardsActive)
            return;
        CardCom card = cardObj as CardCom;
        if (selectedCard == card)
        {
            if (isValidRelease)
            {
                ExeCMD(new ReleaseCardCMD(card.Id));
                card.BanHandle();
            }
            else
            {
                ArrangeCards(arrangeTime);
                card.Reset();
            }
            selectedCard = null;
        }
    }
    private IEnumerator RefillCardsPerformer(RefillCardsArgs args)
    {
        int cnt = args.cardIds.Count;
        const float time1 = 0.2f;
        const float time2 = 0.35f;
        float intervalTime = Mathf.Min(0.1f, cnt / 15f);
        List<CardCom> refillCards = new();
        for (int i = 0; i < args.cardIds.Count; i++)
        {
            CardCom card = NewCard(args.cardIds[i]);
            card.transform.DOMove(discardToPoint.position + new Vector3(0, 1, 0), time1);

            refillCards.Add(card);
            card.transform.DOScale(1, time1).From(0);
        }
        yield return new WaitForSeconds(time1);
        for (int i = 0; i < refillCards.Count; i++)
        {
            refillCards[i].transform.DOMove(drawFromPoint.position + new Vector3(0, 1, 0), time2);
            yield return new WaitForSeconds(intervalTime);
        }
        yield return new WaitForSeconds(time2 - intervalTime);
        for (int i = 0; i < args.cardIds.Count; i++)
        {
            CardCom card = refillCards[i];
            card.transform.DOMove(drawFromPoint.position, time1);
            card.transform.DOScale(0, time1).onComplete = () => RecycleCard(card);
        }

    }
    private IEnumerator DrawActionCardsPerformer(DrawActionCardsArgs args)
    {
        int drawCnt = args.drawed.Count;
        for (int i = 0; i < drawCnt; i++)
        {
            CardCom card = NewCard(args.drawed[i]);
            card.ActiveHandle();
            cards.Add(card);
            card.transform.DOScale(1, arrangeTime).From(0);
            ArrangeCards(arrangeTime);
            yield return new WaitForSeconds(intervalTime);
        }
        yield return new WaitForSeconds(arrangeTime - intervalTime);
    }
    private IEnumerator DrawTradeCardsPerformer(DrawTradeCardsArgs args)
    {
        int drawCnt = args.drawed.Count;
        for (int i = 0; i < drawCnt; i++)
        {
            CardCom card = NewCard(args.drawed[i]);
            card.ActiveHandle();
            cards.Add(card);
            card.transform.DOScale(1, arrangeTime).From(0);
            ArrangeCards(arrangeTime);
            yield return new WaitForSeconds(intervalTime);
        }
        yield return new WaitForSeconds(arrangeTime - intervalTime);
    }
    private bool isValidRelease = false;
    private void Update()
    {
        if (selectedCard != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedCard.SetPosition(new Vector3(mousePos.x, mousePos.y));
            float validY = validHeightPoint.position.y;
            bool isValid = selectedCard.transform.position.y > validY;
            if (!isValidRelease && isValid)
                selectedCard.SetReady();
            else if (isValidRelease && !isValid)
                selectedCard.SetCancel();
            isValidRelease = isValid;
        }
    }
    private IEnumerator EnterActionPhasePerformer()
    {
        phaseState = EViewPhaseState.Action;
        yield return 0;
    }
    private IEnumerator ExitActionPhasePerformer()
    {
        phaseState = EViewPhaseState.None;
        yield return 0;
    }
    private IEnumerator EnterTradePhasePerformer()
    {
        phaseState = EViewPhaseState.Trade;
        yield return 0;
    }
    private IEnumerator ExitTradePhasePerformer()
    {
        phaseState = EViewPhaseState.None;
        yield return 0;
    }
    private IEnumerator DiscardActionCardsPerformer(DiscardActionCardsArgs args)
    {
        List<CardCom> targets = new();
        for (int i = 0; i < args.discards.Count; i++)
            if (cards.Has(args.discards[i]))
                targets.Add(cards[args.discards[i]]);
        for (int i = 0; i < targets.Count; i++)
        {
            CardCom card = targets[i];
            card.transform.DOMove(discardToPoint.position, arrangeTime).SetEase(Ease.OutQuad);
            card.transform.DOScale(0, arrangeTime).SetEase(Ease.OutQuad).onComplete = () =>
            {
                RecycleCard(card);
            };
            cards.Remove(card);
            ArrangeCards(arrangeTime);
            yield return new WaitForSeconds(intervalTime);
        }
        yield return new WaitForSeconds(arrangeTime);
    }
    private IEnumerator DiscardTradeCardsPerformer(DiscardTradeCardsArgs args)
    {
        List<CardCom> targets = new();
        for (int i = 0; i < args.discards.Count; i++)
            if (cards.Has(args.discards[i]))
                targets.Add(cards[args.discards[i]]);
        for (int i = 0; i < targets.Count; i++)
        {
            CardCom card = targets[i];
            card.transform.DOMove(drawFromPoint.position, arrangeTime).SetEase(Ease.OutQuad);
            card.transform.DOScale(0, arrangeTime).SetEase(Ease.OutQuad).onComplete = () =>
            {
                RecycleCard(card);
            };
            cards.Remove(card);
            ArrangeCards(arrangeTime);
            yield return new WaitForSeconds(intervalTime);
        }
        yield return new WaitForSeconds(arrangeTime);

    }
    private IEnumerator CardEffectStartPerformer(CardEffectStartArgs args)
    {
        CardCom card = cards[args.cardId];
        if (!card)
            yield break;
        card.transform.DOScale(1, arrangeTime).SetEase(Ease.OutQuad);
        yield return card.transform.DOMove(effectPoint.position, arrangeTime).SetEase(Ease.OutQuad).WaitForCompletion();
    }
    private void ArrangeCards(float duration)
    {
        if (cards.Count <= 0)
            return;

        const float angleRange = 20;
        const float maxAngleInterval = 4;
        const float maxWid = 7;
        const float maxInterval = 3;
        const float maxHei = 1.5f;
        Vector3 GetPos(int idx)
        {
            if (cards.Count == 1)
                return new Vector3(0, maxHei);
            float stepScale = Math.Min(1, maxInterval * (cards.Count - 1) / maxWid);
            float t = idx * 1f / (cards.Count - 1) - 0.5f;
            t *= stepScale;
            float x = t * maxWid;
            float y = 4 * maxHei * (-t * t + 0.25f);
            return new Vector3(x, y);
        }
        Vector3 GetAng(int idx)
        {
            if (cards.Count == 1)
                return new Vector3(0, 0, 0);
            float half = (cards.Count - 1) / 2f;
            float angleStep = Math.Min(maxAngleInterval, angleRange / (cards.Count - 1));
            float angle = (idx - half) * angleStep;
            return new Vector3(0, 0, -angle);
        }
        for (int i = 0; i < cards.Count; i++)
        {
            cards.Index(i).transform.DOMove(GetPos(i) + handFolder.position, duration).SetEase(Ease.OutQuad);
            cards.Index(i).transform.DORotate(GetAng(i) + handFolder.position, duration).SetEase(Ease.OutQuad);
        }
    }
}