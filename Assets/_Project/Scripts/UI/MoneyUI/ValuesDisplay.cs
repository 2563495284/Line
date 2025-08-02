using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ValuesDisplay : MonoBehaviour
{
    [Header("UI组件")]
    public TextMeshProUGUI valuesText;
    public TextMeshProUGUI changeText;

    [Header("显示设置")]
    public string valuesFormat = "N0";
    public bool showChange = true;
    public float changeAnimationDuration = 0.5f;

    [Header("颜色设置")]
    public Color positiveChangeColor = Color.green;
    public Color negativeChangeColor = Color.red;
    public Color neutralColor = Color.white;

    [Header("动画设置")]
    public bool enableCountAnimation = true;
    public float countAnimationDuration = 1f;

    private float currentValues = 0;
    private float previousValues = 0;
    private Coroutine changeAnimationCoroutine;
    private Coroutine countAnimationCoroutine;

    public float CurrentValues { get { return currentValues; } }

    void Start()
    {
        InitializeDisplay();
    }

    void InitializeDisplay()
    {
        if (valuesText != null)
        {
            valuesText.text = "0";
        }

        if (changeText != null)
        {
            changeText.text = "";
            changeText.gameObject.SetActive(showChange);
        }
    }

    public void UpdateValues(float newValues)
    {
        previousValues = currentValues;
        currentValues = newValues;

        if (enableCountAnimation)
        {
            StartCountAnimation();
        }
        else
        {
            UpdateValuesText();
        }

        if (showChange && previousValues != 0)
        {
            ShowValuesChange();
        }
    }

    void UpdateValuesText()
    {
        if (valuesText != null)
        {
            valuesText.text = currentValues.ToString(valuesFormat);
        }
    }

    void StartCountAnimation()
    {
        if (countAnimationCoroutine != null)
        {
            StopCoroutine(countAnimationCoroutine);
        }

        countAnimationCoroutine = StartCoroutine(AnimateValuesCount());
    }

    IEnumerator AnimateValuesCount()
    {
        float elapsed = 0f;
        float startValues = previousValues;
        float targetValues = currentValues;

        while (elapsed < countAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / countAnimationDuration;

            // 使用缓动函数使动画更自然
            t = Mathf.SmoothStep(0f, 1f, t);

            float currentValue = Mathf.RoundToInt(Mathf.Lerp(startValues, targetValues, t));

            if (valuesText != null)
            {
                valuesText.text = currentValue.ToString(valuesFormat);
            }

            yield return null;
        }

        // 确保最终值正确
        UpdateValuesText();
    }

    void ShowValuesChange()
    {
        float change = currentValues - previousValues;

        if (changeText != null)
        {
            string changeString = "";
            Color changeColor = neutralColor;

            if (change > 0)
            {
                changeString = $"+{change.ToString(valuesFormat)}";
                changeColor = positiveChangeColor;
            }
            else if (change < 0)
            {
                changeString = change.ToString(valuesFormat);
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
            changeAnimationCoroutine = StartCoroutine(AnimateValuesChange(changeColor));
        }
    }

    IEnumerator AnimateValuesChange(Color targetColor)
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
    public void SetValuesFormat(string format)
    {
        valuesFormat = format;
        UpdateValuesText();
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
    public float GetCurrentValues()
    {
        return currentValues;
    }

    // 获取金钱变化
    public float GetValuesChange()
    {
        return currentValues - previousValues;
    }

    // 格式化金钱显示（添加千位分隔符等）
    public string FormatValues(float amount)
    {
        return amount.ToString(valuesFormat);
    }

    // 检查是否有足够的金钱
    public bool HasEnoughValues(float requiredAmount)
    {
        return currentValues >= requiredAmount;
    }
}