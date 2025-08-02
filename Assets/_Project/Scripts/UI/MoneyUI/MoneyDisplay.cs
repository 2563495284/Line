using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    [Header("UI组件")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI changeText;

    [Header("显示设置")]
    public string moneyFormat = "N0";
    public bool showChange = true;
    public float changeAnimationDuration = 0.5f;

    [Header("颜色设置")]
    public Color positiveChangeColor = Color.green;
    public Color negativeChangeColor = Color.red;
    public Color neutralColor = Color.white;

    [Header("动画设置")]
    public bool enableCountAnimation = true;
    public float countAnimationDuration = 1f;

    private float currentMoney = 0;
    private float previousMoney = 0;
    private Coroutine changeAnimationCoroutine;
    private Coroutine countAnimationCoroutine;

    public float CurrentMoney { get { return currentMoney; } }

    void Start()
    {
        InitializeDisplay();
    }

    void InitializeDisplay()
    {
        if (moneyText != null)
        {
            moneyText.text = "0";
        }

        if (changeText != null)
        {
            changeText.text = "";
            changeText.gameObject.SetActive(showChange);
        }
    }

    public void UpdateMoney(float newMoney)
    {
        previousMoney = currentMoney;
        currentMoney = newMoney;

        if (enableCountAnimation)
        {
            StartCountAnimation();
        }
        else
        {
            UpdateMoneyText();
        }

        if (showChange && previousMoney != 0)
        {
            ShowMoneyChange();
        }
    }

    void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString(moneyFormat);
        }
    }

    void StartCountAnimation()
    {
        if (countAnimationCoroutine != null)
        {
            StopCoroutine(countAnimationCoroutine);
        }

        countAnimationCoroutine = StartCoroutine(AnimateMoneyCount());
    }

    IEnumerator AnimateMoneyCount()
    {
        float elapsed = 0f;
        float startMoney = previousMoney;
        float targetMoney = currentMoney;

        while (elapsed < countAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / countAnimationDuration;

            // 使用缓动函数使动画更自然
            t = Mathf.SmoothStep(0f, 1f, t);

            float currentValue = Mathf.RoundToInt(Mathf.Lerp(startMoney, targetMoney, t));

            if (moneyText != null)
            {
                moneyText.text = currentValue.ToString(moneyFormat);
            }

            yield return null;
        }

        // 确保最终值正确
        UpdateMoneyText();
    }

    void ShowMoneyChange()
    {
        float change = currentMoney - previousMoney;

        if (changeText != null)
        {
            string changeString = "";
            Color changeColor = neutralColor;

            if (change > 0)
            {
                changeString = $"+{change.ToString(moneyFormat)}";
                changeColor = positiveChangeColor;
            }
            else if (change < 0)
            {
                changeString = change.ToString(moneyFormat);
                changeColor = negativeChangeColor;
            }
            else
            {
                changeString = "0";
                changeColor = neutralColor;
            }

            changeText.text = changeString;
            changeText.color = changeColor;

            // 启动变化动画
            if (changeAnimationCoroutine != null)
            {
                StopCoroutine(changeAnimationCoroutine);
            }
            changeAnimationCoroutine = StartCoroutine(AnimateMoneyChange(changeColor));
        }
    }

    IEnumerator AnimateMoneyChange(Color targetColor)
    {

        float elapsed = 0f;

        while (elapsed < changeAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / changeAnimationDuration;


            yield return null;
        }

        // 保持颜色一段时间
        yield return new WaitForSeconds(0.5f);

        // 恢复原始颜色
        elapsed = 0f;
        while (elapsed < changeAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / changeAnimationDuration;

            yield return null;
        }
    }

    // 设置金钱格式
    public void SetMoneyFormat(string format)
    {
        moneyFormat = format;
        UpdateMoneyText();
    }

    // 设置是否显示变化
    public void SetShowChange(bool show)
    {
        showChange = show;
        if (changeText != null)
        {
            changeText.gameObject.SetActive(show);
        }
    }

    // 设置是否启用计数动画
    public void SetCountAnimation(bool enable)
    {
        enableCountAnimation = enable;
    }

    // 设置颜色
    public void SetPositiveChangeColor(Color color)
    {
        positiveChangeColor = color;
    }

    public void SetNegativeChangeColor(Color color)
    {
        negativeChangeColor = color;
    }

    // 获取当前金钱
    public float GetCurrentMoney()
    {
        return currentMoney;
    }

    // 获取金钱变化
    public float GetMoneyChange()
    {
        return currentMoney - previousMoney;
    }

    // 格式化金钱显示（添加千位分隔符等）
    public string FormatMoney(float amount)
    {
        return amount.ToString(moneyFormat);
    }

    // 检查是否有足够的金钱
    public bool HasEnoughMoney(float requiredAmount)
    {
        return currentMoney >= requiredAmount;
    }
}