using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TipsUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;


    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Coroutine hideCoroutine;
    private Sequence currentAnimation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // 如果没有指定CanvasGroup，自动获取
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // 设置初始状态
        canvasGroup.alpha = 0f;
        originalPosition = rectTransform.anchoredPosition;
    }

    /// <summary>
    /// 显示提示
    /// </summary>
    public void ShowTip(string message, TipsType tipsType, float duration, float fadeInDuration, float fadeOutDuration, float moveDistance)
    {
        Debug.Log($"TipsUI.ShowTip: '{message}', 持续时间: {duration}");

        // 停止之前的动画和协程
        StopCurrentAnimation();
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // 重置状态
        ResetState();

        // 设置消息和样式
        SetMessage(message);
        SetStyle(tipsType);

        // 重置位置
        rectTransform.anchoredPosition = originalPosition;

        // 开始显示动画
        StartShowAnimation(fadeInDuration, moveDistance);

        // 设置自动隐藏
        hideCoroutine = StartCoroutine(AutoHideCoroutine(duration, fadeOutDuration, moveDistance));
    }

    /// <summary>
    /// 重置TipsUI状态
    /// </summary>
    private void ResetState()
    {
        // 确保CanvasGroup处于正确状态
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        // 重置位置
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originalPosition;
        }

        Debug.Log("TipsUI状态已重置");
    }

    /// <summary>
    /// 隐藏提示
    /// </summary>
    public void HideTip()
    {
        Debug.Log("TipsUI.HideTip: 开始隐藏提示");

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        StartHideAnimation(0.3f, 50f);
    }

    /// <summary>
    /// 设置消息内容
    /// </summary>
    private void SetMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    /// <summary>
    /// 设置样式
    /// </summary>
    private void SetStyle(TipsType tipsType)
    {
        Color backgroundColor = GetColorByType(tipsType);

        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }

        // 可以在这里设置图标
        if (iconImage != null)
        {
            // 根据类型设置不同的图标
            // iconImage.sprite = GetIconByType(tipsType);
        }
    }

    /// <summary>
    /// 根据类型获取颜色
    /// </summary>
    private Color GetColorByType(TipsType tipsType)
    {
        switch (tipsType)
        {
            case TipsType.Success:
                return new Color(0.2f, 0.8f, 0.2f, 0.9f);
            case TipsType.Warning:
                return new Color(1f, 0.6f, 0.2f, 0.9f);
            case TipsType.Error:
                return new Color(1f, 0.2f, 0.2f, 0.9f);
            case TipsType.Info:
            default:
                return new Color(0.2f, 0.6f, 1f, 0.9f);
        }
    }

    /// <summary>
    /// 开始显示动画
    /// </summary>
    private void StartShowAnimation(float fadeInDuration, float moveDistance)
    {
        currentAnimation = DOTween.Sequence();

        // 淡入动画
        currentAnimation.Append(canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad));

        // 向上移动动画
        Vector3 targetPosition = originalPosition + Vector3.up * moveDistance;
        currentAnimation.Join(rectTransform.DOAnchorPos(targetPosition, fadeInDuration).SetEase(Ease.OutQuad));

        currentAnimation.OnComplete(() => currentAnimation = null);
    }

    /// <summary>
    /// 开始隐藏动画
    /// </summary>
    private void StartHideAnimation(float fadeOutDuration, float moveDistance)
    {
        currentAnimation = DOTween.Sequence();

        // 淡出动画
        currentAnimation.Append(canvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad));

        // 向上移动动画
        Vector3 targetPosition = rectTransform.anchoredPosition + Vector2.up * moveDistance;
        currentAnimation.Join(rectTransform.DOAnchorPos(targetPosition, fadeOutDuration).SetEase(Ease.InQuad));

        currentAnimation.OnComplete(() =>
        {
            currentAnimation = null;
            Debug.Log("TipsUI隐藏动画完成，准备回收");
            if (TipsSystem.Ins != null)
            {
                TipsSystem.Ins.RecycleTipsUI(this);
            }
            else
            {
                Debug.LogError("TipsSystem.Instance为null，无法回收TipsUI！");
            }
        });
    }

    /// <summary>
    /// 自动隐藏协程
    /// </summary>
    private IEnumerator AutoHideCoroutine(float duration, float fadeOutDuration, float moveDistance)
    {
        Debug.Log($"AutoHideCoroutine开始等待 {duration} 秒");
        yield return new WaitForSeconds(duration);

        Debug.Log($"AutoHideCoroutine等待完成，GameObject活跃状态: {gameObject.activeInHierarchy}");

        if (gameObject.activeInHierarchy)
        {
            Debug.Log("开始自动隐藏动画");
            StartHideAnimation(fadeOutDuration, moveDistance);
        }
        else
        {
            Debug.Log("GameObject已不活跃，跳过隐藏动画");
        }

        hideCoroutine = null;
    }

    /// <summary>
    /// 停止当前动画
    /// </summary>
    private void StopCurrentAnimation()
    {
        if (currentAnimation != null)
        {
            currentAnimation.Kill();
            currentAnimation = null;
        }
    }

    private void OnDestroy()
    {
        StopCurrentAnimation();
    }
}