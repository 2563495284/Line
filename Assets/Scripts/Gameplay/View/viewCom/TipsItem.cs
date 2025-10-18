using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TipsItem : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private TextMeshPro messageText;
    [SerializeField] private SpriteRenderer backgroundImage;


    private Coroutine hideCoroutine;
    private Sequence currentAnimation;
    private float moveDistance = 1;
    private void Awake()
    {
        SetAlpha(0);
    }
    private void SetAlpha(float alpha)
    {
        void SetAlphaDfs(Transform root, float alpha)
        {
            TextMeshPro tmp = root.GetComponent<TextMeshPro>();
            if (tmp != null)
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, alpha);
            SpriteRenderer sprr = root.GetComponent<SpriteRenderer>();
            if (sprr != null)
                sprr.color = new Color(sprr.color.r, sprr.color.g, sprr.color.b, alpha);
            for (int i = 0; i < root.childCount; i++)
                SetAlphaDfs(root.GetChild(i), alpha);
        }
        SetAlphaDfs(transform, alpha);
    }
    /// <summary>
    /// 显示提示
    /// </summary>
    public void ShowTip(string message, TipsType tipsType, float duration, float fadeInDuration, float fadeOutDuration, float moveDistance)
    {
        this.moveDistance = moveDistance;
        // 停止之前的动画和协程
        StopCurrentAnimation();
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // 重置状态
        SetAlpha(0);
        transform.localPosition = new Vector2(0, 0);


        // 设置消息和样式
        messageText.text = message;
        backgroundImage.color = GetColorByType(tipsType);

        // 重置位置
        transform.localPosition = new Vector2(0, 0);

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
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
        StartHideAnimation(0.3f, moveDistance);
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
        float alpha = 0;
        DOTween.To(() => alpha, a =>
        {
            alpha = a;
            SetAlpha(a);
        }, 1, fadeInDuration).SetEase(Ease.OutQuad);
        // 向上移动动画
        Vector3 targetPosition = Vector3.up * moveDistance;
        currentAnimation.Join(transform.DOLocalMove(targetPosition, fadeInDuration).SetEase(Ease.OutQuad));

        currentAnimation.OnComplete(() => currentAnimation = null);
    }

    /// <summary>
    /// 开始隐藏动画
    /// </summary>
    private void StartHideAnimation(float fadeOutDuration, float moveDistance)
    {
        currentAnimation = DOTween.Sequence();

        float alpha = 1;
        DOTween.To(() => alpha, a =>
        {
            alpha = a;
            SetAlpha(a);
        }, 0, fadeOutDuration).SetEase(Ease.OutQuad);
        // 向上移动动画
        Vector3 targetPosition = transform.localPosition + Vector3.up * moveDistance;
        currentAnimation.Join(transform.DOLocalMove(targetPosition, fadeOutDuration).SetEase(Ease.InQuad));

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