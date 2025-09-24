using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TipsCom : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image backgroundImage;


    private RectTransform rectTransform;
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
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = new Vector2(0, 0);


        // 设置消息和样式
        messageText.text = message;
        backgroundImage.color = GetColorByType(tipsType);

        // 重置位置
        rectTransform.anchoredPosition = new Vector2(0, 0);

        // 开始显示动画
        StartShowAnimation(fadeInDuration, moveDistance);

        // 设置自动隐藏
        hideCoroutine = StartCoroutine(AutoHideCoroutine(duration, fadeOutDuration, moveDistance));
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
        Vector3 targetPosition = Vector3.up * moveDistance;
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
            gameObject.OPPush();
        });
    }

    /// <summary>
    /// 自动隐藏协程
    /// </summary>
    private IEnumerator AutoHideCoroutine(float duration, float fadeOutDuration, float moveDistance)
    {
        yield return new WaitForSeconds(duration);
        if (gameObject.activeInHierarchy)
            StartHideAnimation(fadeOutDuration, moveDistance);

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