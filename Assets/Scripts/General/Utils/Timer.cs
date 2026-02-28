using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [Header("计时器设置")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private bool isCountdown = true; // true为倒计时，false为正计时
    [SerializeField] private bool autoStart = false;
    [SerializeField] private bool loop = false;

    [Header("事件")]
    [SerializeField] private UnityEvent onTimerComplete;
    [SerializeField] private UnityEvent<float> onTimerTick; // 传递当前时间

    private float currentTime;
    private bool isRunning = false;
    private bool isPaused = false;
    private Coroutine timerCoroutine;

    public float CurrentTime => currentTime;
    public float Duration => duration;
    public bool IsRunning => isRunning;
    public bool IsPaused => isPaused;
    public bool IsComplete => isCountdown ? currentTime <= 0 : currentTime >= duration;

    public event Action OnTimerComplete;
    public event Action<float> OnTimerTick;

    private void Start()
    {
        if (autoStart)
        {
            StartTimer();
        }
    }

    /// <summary>
    /// 启动计时器
    /// </summary>
    public void StartTimer()
    {
        if (isRunning) return;

        ResetTimer();
        isRunning = true;
        isPaused = false;
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    /// <summary>
    /// 启动计时器（指定持续时间）
    /// </summary>
    public void StartTimer(float newDuration)
    {
        duration = newDuration;
        StartTimer();
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    public void StopTimer()
    {
        if (!isRunning) return;

        isRunning = false;
        isPaused = false;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    /// <summary>
    /// 暂停计时器
    /// </summary>
    public void PauseTimer()
    {
        if (!isRunning || isPaused) return;
        isPaused = true;
    }

    /// <summary>
    /// 恢复计时器
    /// </summary>
    public void ResumeTimer()
    {
        if (!isRunning || !isPaused) return;
        isPaused = false;
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        currentTime = isCountdown ? duration : 0f;
    }

    /// <summary>
    /// 设置持续时间
    /// </summary>
    public void SetDuration(float newDuration)
    {
        duration = newDuration;
        if (!isRunning)
        {
            ResetTimer();
        }
    }

    /// <summary>
    /// 设置是否为倒计时
    /// </summary>
    public void SetCountdown(bool countdown)
    {
        isCountdown = countdown;
        if (!isRunning)
        {
            ResetTimer();
        }
    }

    /// <summary>
    /// 设置是否循环
    /// </summary>
    public void SetLoop(bool shouldLoop)
    {
        loop = shouldLoop;
    }

    /// <summary>
    /// 获取剩余时间（倒计时）或已过时间（正计时）
    /// </summary>
    public float GetRemainingTime()
    {
        return isCountdown ? currentTime : duration - currentTime;
    }

    /// <summary>
    /// 获取进度（0-1）
    /// </summary>
    public float GetProgress()
    {
        return isCountdown ? 1f - (currentTime / duration) : currentTime / duration;
    }

    private IEnumerator TimerCoroutine()
    {
        while (isRunning)
        {
            if (!isPaused)
            {
                if (isCountdown)
                {
                    currentTime -= Time.deltaTime;
                    if (currentTime <= 0)
                    {
                        currentTime = 0;
                        CompleteTimer();
                        if (!loop) break;
                        ResetTimer();
                    }
                }
                else
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= duration)
                    {
                        currentTime = duration;
                        CompleteTimer();
                        if (!loop) break;
                        ResetTimer();
                    }
                }

                // 触发tick事件
                onTimerTick?.Invoke(currentTime);
                OnTimerTick?.Invoke(currentTime);
            }

            yield return null;
        }

        timerCoroutine = null;
    }

    private void CompleteTimer()
    {
        onTimerComplete?.Invoke();
        OnTimerComplete?.Invoke();
    }

    private void OnDestroy()
    {
        StopTimer();
    }

    private void OnDisable()
    {
        StopTimer();
    }
}