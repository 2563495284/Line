using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class StockDisplay : MonoBehaviour
{
    [Header("UI组件")]
    public TextMeshProUGUI stockText;
    public TextMeshProUGUI changeText;

    [Header("显示设置")]
    public string stockFormat = "N0";
    public bool showChange = true;
    public float changeAnimationDuration = 0.5f;

    [Header("颜色设置")]
    public Color positiveChangeColor = Color.green;
    public Color negativeChangeColor = Color.red;
    public Color neutralColor = Color.white;

    [Header("动画设置")]
    public bool enableCountAnimation = true;
    public float countAnimationDuration = 1f;

    private int currentStock = 0;
    private int previousStock = 0;
    private Coroutine changeAnimationCoroutine;
    private Coroutine countAnimationCoroutine;

    public float CurrentStock { get { return currentStock; } }

    void Start()
    {
        InitializeDisplay();
    }

    void InitializeDisplay()
    {
        if (stockText != null)
        {
            stockText.text = $"石油: {0.ToString(stockFormat)}";
        }

        if (changeText != null)
        {
            changeText.text = "";
            changeText.gameObject.SetActive(showChange);
        }
    }

    public void UpdateStock(int newStock)
    {
        previousStock = currentStock;
        currentStock = newStock;

        if (enableCountAnimation)
        {
            StartCountAnimation();
        }
        else
        {
            UpdateStockText();
        }

        if (showChange && previousStock != 0)
        {
            ShowStockChange();
        }
    }

    void UpdateStockText()
    {
        if (stockText != null)
        {
            stockText.text = $"石油: {currentStock.ToString(stockFormat)}";
        }
    }

    void StartCountAnimation()
    {
        if (countAnimationCoroutine != null)
        {
            StopCoroutine(countAnimationCoroutine);
        }

        countAnimationCoroutine = StartCoroutine(AnimateStockCount());
    }

    IEnumerator AnimateStockCount()
    {
        float elapsed = 0f;
        float startStock = previousStock;
        float targetStock = currentStock;

        while (elapsed < countAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / countAnimationDuration;

            // 使用缓动函数使动画更自然
            t = Mathf.SmoothStep(0f, 1f, t);

            float currentValue = Mathf.RoundToInt(Mathf.Lerp(startStock, targetStock, t));

            if (stockText != null)
            {
                stockText.text = $"石油: {currentValue.ToString(stockFormat)}";
            }

            yield return null;
        }

        // 确保最终值正确
        UpdateStockText();
    }

    void ShowStockChange()
    {
        float change = currentStock - previousStock;

        if (changeText != null)
        {
            string changeString = "";
            Color changeColor = neutralColor;

            if (change > 0)
            {
                changeString = $"+{change.ToString(stockFormat)}";
                changeColor = positiveChangeColor;
            }
            else if (change < 0)
            {
                changeString = change.ToString(stockFormat);
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
            changeAnimationCoroutine = StartCoroutine(AnimateStockChange(changeColor));
        }
    }

    IEnumerator AnimateStockChange(Color targetColor)
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
    public void SetStockFormat(string format)
    {
        stockFormat = format;
        UpdateStockText();
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
    public float GetCurrentStock()
    {
        return currentStock;
    }

    // 获取金钱变化
    public float GetStockChange()
    {
        return currentStock - previousStock;
    }

    // 格式化金钱显示（添加千位分隔符等）
    public string FormatStock(float amount)
    {
        return amount.ToString(stockFormat);
    }

    // 检查是否有足够的金钱
    public bool HasEnoughStock(float requiredAmount)
    {
        return currentStock >= requiredAmount;
    }
}