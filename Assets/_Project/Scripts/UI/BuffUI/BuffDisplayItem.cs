using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// 单个Buff的UI显示组件
/// </summary>
public class BuffDisplayItem : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI buffNameText;
    [SerializeField] private TextMeshProUGUI stackCountText;
    [SerializeField] private TextMeshProUGUI roundsText;
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject stackCountContainer;
    [SerializeField] private GameObject roundsContainer;

    [Header("动画设置")]
    [SerializeField] private float pulseScale = 1.1f;
    [SerializeField] private float pulseDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("颜色设置")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor = Color.red;
    [SerializeField] private Color expiredColor = Color.gray;

    private BuffUIData currentData;
    private Sequence pulseSequence;
    private Sequence shakeSequence;
    private bool isAnimating = false;

    #region Public Methods

    /// <summary>
    /// 更新Buff显示
    /// </summary>
    public void UpdateBuff(BuffUIData data)
    {
        if (data == null) return;

        bool isNewBuff = currentData == null || !currentData.isActive;
        currentData = data;

        // 更新基础信息
        UpdateBasicInfo();

        // 更新数值信息
        UpdateValues();

        // 更新颜色
        UpdateColors();

        // 更新进度条
        UpdateProgressBar();

        // 播放动画
        if (isNewBuff && data.isActive)
        {
            PlayAppearAnimation();
        }
        else if (data.shouldPulse)
        {
            PlayPulseAnimation();
        }

        // 设置可见性
        gameObject.SetActive(data.isActive);
    }

    /// <summary>
    /// 播放消失动画
    /// </summary>
    public void PlayDisappearAnimation(System.Action onComplete = null)
    {
        if (isAnimating) return;

        isAnimating = true;
        transform.DOScale(0f, fadeOutDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                isAnimating = false;
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// 重置显示项
    /// </summary>
    public void ResetItem()
    {
        StopAllAnimations();
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
        currentData = null;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 更新基础信息
    /// </summary>
    private void UpdateBasicInfo()
    {
        if (buffNameText != null)
        {
            buffNameText.text = currentData.GetDisplayText();
        }

        if (iconImage != null && currentData.icon != null)
        {
            iconImage.sprite = currentData.icon;
            iconImage.gameObject.SetActive(true);
        }
        else if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 更新数值信息
    /// </summary>
    private void UpdateValues()
    {
        // 更新层数显示
        if (stackCountText != null && stackCountContainer != null)
        {
            if (currentData.stackCount > 1)
            {
                stackCountText.text = currentData.stackCount.ToString();
                stackCountContainer.SetActive(true);
            }
            else
            {
                stackCountContainer.SetActive(false);
            }
        }

        // 更新回合显示
        if (roundsText != null && roundsContainer != null)
        {
            string roundsDisplay = currentData.GetRoundsText();
            if (!string.IsNullOrEmpty(roundsDisplay))
            {
                roundsText.text = roundsDisplay;
                roundsContainer.SetActive(true);
            }
            else
            {
                roundsContainer.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 更新颜色
    /// </summary>
    private void UpdateColors()
    {
        Color targetColor = normalColor;

        if (!currentData.isActive)
        {
            targetColor = expiredColor;
        }
        else if (currentData.remainingRounds <= 1)
        {
            targetColor = dangerColor;
        }
        else if (currentData.remainingRounds <= 2)
        {
            targetColor = warningColor;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = targetColor;
        }

        if (buffNameText != null)
        {
            buffNameText.color = currentData.textColor;
        }
    }

    /// <summary>
    /// 更新进度条
    /// </summary>
    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = currentData.progress;

            // 根据进度设置进度条颜色
            Color progressColor = Color.Lerp(dangerColor, normalColor, currentData.progress);
            progressBar.color = progressColor;
        }
    }

    /// <summary>
    /// 播放出现动画
    /// </summary>
    private void PlayAppearAnimation()
    {
        if (isAnimating) return;

        isAnimating = true;
        transform.localScale = Vector3.zero;

        transform.DOScale(1f, fadeInDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => isAnimating = false);
    }

    /// <summary>
    /// 播放脉冲动画
    /// </summary>
    private void PlayPulseAnimation()
    {
        if (pulseSequence != null && pulseSequence.IsActive()) return;

        pulseSequence = DOTween.Sequence()
            .Append(transform.DOScale(pulseScale, pulseDuration * 0.5f))
            .Append(transform.DOScale(1f, pulseDuration * 0.5f))
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// 播放震动动画
    /// </summary>
    private void PlayShakeAnimation()
    {
        if (shakeSequence != null && shakeSequence.IsActive()) return;

        Vector3 originalPos = transform.localPosition;
        shakeSequence = DOTween.Sequence()
            .Append(transform.DOShakePosition(0.5f, 10f, 20, 90, false, true))
            .OnComplete(() => transform.localPosition = originalPos);
    }

    /// <summary>
    /// 停止所有动画
    /// </summary>
    private void StopAllAnimations()
    {
        pulseSequence?.Kill();
        shakeSequence?.Kill();
        transform.DOKill();
        isAnimating = false;
    }

    #endregion

    #region Unity Lifecycle

    private void OnDestroy()
    {
        StopAllAnimations();
    }

    private void OnDisable()
    {
        StopAllAnimations();
    }

    #endregion
}
