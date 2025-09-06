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

    [SerializeField] private float nextRoundButtonCooldown = 0.5f; // 冷却时间（秒）
    private bool isNextRoundButtonOnCooldown = false; // 是否处于冷却状态
    private Coroutine cooldownCoroutine; // 冷却协程引用
    private string originalButtonText; // 原始按钮文本

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

        // 保存原始按钮文本
        var buttonText = nextRoundButton.GetComponentInChildren<UnityEngine.UI.Text>();
        if (buttonText != null)
        {
            originalButtonText = buttonText.text;
        }
        else
        {
            // 如果使用的是TextMeshPro
            var buttonTMP = nextRoundButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (buttonTMP != null)
            {
                originalButtonText = buttonTMP.text;
            }
        }
    }
    void OnDestroy()
    {
        nextRoundButton.onClick.RemoveListener(OnNextRoundButtonClick);

        // 停止冷却协程
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
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
        // 检查是否处于冷却状态
        if (isNextRoundButtonOnCooldown)
        {
            return; // 如果在冷却中，直接返回，不执行操作
        }
        if (ActionSystem.Instance.IsPerforming) return;

        // 执行原有逻辑
        NextRoundTurnGA nextRoundTurnGA = new();
        ActionSystem.Instance.Perform(nextRoundTurnGA);

        // 开始冷却
        StartButtonCooldown();
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

    #region 按钮冷却系统

    /// <summary>
    /// 开始按钮冷却
    /// </summary>
    private void StartButtonCooldown()
    {
        if (isNextRoundButtonOnCooldown)
            return;

        // 停止之前的冷却协程（如果有的话）
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        // 开始新的冷却
        cooldownCoroutine = StartCoroutine(ButtonCooldownCoroutine());
    }

    /// <summary>
    /// 按钮冷却倒计时协程
    /// </summary>
    private IEnumerator ButtonCooldownCoroutine()
    {
        isNextRoundButtonOnCooldown = true;
        nextRoundButton.interactable = false;

        float remainingTime = nextRoundButtonCooldown;

        while (remainingTime > 0)
        {
            // 更新按钮文本显示剩余冷却时间
            UpdateButtonText($"{originalButtonText} ({remainingTime:F1}s)");

            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // 冷却结束
        isNextRoundButtonOnCooldown = false;
        nextRoundButton.interactable = true;
        UpdateButtonText(originalButtonText);
        cooldownCoroutine = null;
    }

    /// <summary>
    /// 更新按钮文本
    /// </summary>
    private void UpdateButtonText(string text)
    {
        return;
        var buttonText = nextRoundButton.GetComponentInChildren<UnityEngine.UI.Text>();
        if (buttonText != null)
        {
            buttonText.text = text;
        }
        else
        {
            // 如果使用的是TextMeshPro
            var buttonTMP = nextRoundButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (buttonTMP != null)
            {
                buttonTMP.text = text;
            }
        }
    }

    /// <summary>
    /// 设置按钮冷却时间
    /// </summary>
    /// <param name="cooldownTime">冷却时间（秒）</param>
    public void SetButtonCooldown(float cooldownTime)
    {
        nextRoundButtonCooldown = Mathf.Max(0f, cooldownTime);
    }

    /// <summary>
    /// 停止按钮冷却
    /// </summary>
    public void StopButtonCooldown()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }

        isNextRoundButtonOnCooldown = false;
        nextRoundButton.interactable = true;
        UpdateButtonText(originalButtonText);
    }

    /// <summary>
    /// 获取当前是否处于冷却状态
    /// </summary>
    /// <returns>是否处于冷却状态</returns>
    public bool IsButtonOnCooldown()
    {
        return isNextRoundButtonOnCooldown;
    }

    #endregion

}