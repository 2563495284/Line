using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class NewsUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;

    [Header("颜色设置")]
    [SerializeField] private Color infoColor = new Color(0.2f, 0.6f, 1f, 0.9f);
    [SerializeField] private Color cardPlayColor = new Color(0.8f, 0.4f, 1f, 0.9f);
    [SerializeField] private Color tradeColor = new Color(1f, 0.8f, 0.2f, 0.9f);
    [SerializeField] private Color positiveColor = new Color(0.2f, 0.8f, 0.2f, 0.9f);
    [SerializeField] private Color negativeColor = new Color(1f, 0.2f, 0.2f, 0.9f);

    private RectTransform rectTransform;
    private Vector3 targetPosition;
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

        // 设置锚点到右上角
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);

        // 设置初始位置
        rectTransform.anchoredPosition = Vector2.zero;

        Debug.Log($"NewsUI初始化完成，锚点: {rectTransform.anchorMin}, 轴心: {rectTransform.pivot}, 位置: {rectTransform.anchoredPosition}");
    }

    /// <summary>
    /// 显示新闻
    /// </summary>
    public void ShowNews(string title, string content, NewsType newsType, float duration, float fadeInDuration, float fadeOutDuration, float slideDistance)
    {
        // 停止之前的动画
        StopCurrentAnimation();

        // 设置新闻内容
        SetNewsContent(title, content);
        SetNewsStyle(newsType);

        // 设置初始位置（屏幕外）
        Vector3 startPosition = new Vector3(slideDistance, 0, 0);
        rectTransform.anchoredPosition = startPosition;

        Debug.Log($"新闻显示: {title}, 初始位置: {startPosition}, 目标位置: {targetPosition}");

        // 开始显示动画
        StartShowAnimation(fadeInDuration, slideDistance);

        // 设置自动隐藏
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(AutoHideCoroutine(duration, fadeOutDuration, slideDistance));
    }

    /// <summary>
    /// 隐藏新闻
    /// </summary>
    public void HideNews()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        StartHideAnimation(0.3f, 100f);
    }

    /// <summary>
    /// 设置目标位置
    /// </summary>
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        Debug.Log($"设置目标位置: {position}, 当前位置: {rectTransform.anchoredPosition}");
        rectTransform.DOAnchorPos(position, 0.3f).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 设置新闻内容
    /// </summary>
    private void SetNewsContent(string title, string content)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (contentText != null)
        {
            contentText.text = content;
        }
    }

    /// <summary>
    /// 设置新闻样式
    /// </summary>
    private void SetNewsStyle(NewsType newsType)
    {
        Color backgroundColor = GetColorByType(newsType);

        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }

        // 可以在这里设置图标
        if (iconImage != null)
        {
            // 根据类型设置不同的图标
            // iconImage.sprite = GetIconByType(newsType);
        }
    }

    /// <summary>
    /// 根据类型获取颜色
    /// </summary>
    private Color GetColorByType(NewsType newsType)
    {
        switch (newsType)
        {
            case NewsType.CardPlay:
                return cardPlayColor;
            case NewsType.Trade:
                return tradeColor;
            case NewsType.Positive:
                return positiveColor;
            case NewsType.Negative:
                return negativeColor;
            case NewsType.Info:
            default:
                return infoColor;
        }
    }

    /// <summary>
    /// 开始显示动画
    /// </summary>
    private void StartShowAnimation(float fadeInDuration, float slideDistance)
    {
        currentAnimation = DOTween.Sequence();

        // 淡入动画
        currentAnimation.Append(canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad));

        // 从右向左滑入动画
        Vector3 endPosition = targetPosition;
        currentAnimation.Join(rectTransform.DOAnchorPos(endPosition, fadeInDuration).SetEase(Ease.OutQuad));

        currentAnimation.OnComplete(() => currentAnimation = null);
    }

    /// <summary>
    /// 开始隐藏动画
    /// </summary>
    private void StartHideAnimation(float fadeOutDuration, float slideDistance)
    {
        currentAnimation = DOTween.Sequence();

        // 淡出动画
        currentAnimation.Append(canvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad));

        // 向右滑出动画
        Vector3 endPosition = rectTransform.anchoredPosition + Vector2.right * slideDistance;
        currentAnimation.Join(rectTransform.DOAnchorPos(endPosition, fadeOutDuration).SetEase(Ease.InQuad));

        currentAnimation.OnComplete(() =>
        {
            currentAnimation = null;
            NewsSystem.Instance.RecycleNewsUI(this);
        });
    }

    /// <summary>
    /// 自动隐藏协程
    /// </summary>
    private IEnumerator AutoHideCoroutine(float duration, float fadeOutDuration, float slideDistance)
    {
        yield return new WaitForSeconds(duration);

        if (gameObject.activeInHierarchy)
        {
            StartHideAnimation(fadeOutDuration, slideDistance);
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