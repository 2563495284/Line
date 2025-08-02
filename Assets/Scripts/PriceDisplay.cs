using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PriceDisplay : MonoBehaviour
{
    [Header("UI组件")]
    public Text priceText;
    public Text changeText;
    public Image backgroundImage;

    [Header("显示设置")]
    public string priceFormat = "F2";
    public bool showChange = true;
    public float changeAnimationDuration = 0.5f;

    [Header("颜色设置")]
    public Color positiveChangeColor = Color.green;
    public Color negativeChangeColor = Color.red;
    public Color neutralColor = Color.white;

    private float currentPrice = 0f;
    private float previousPrice = 0f;
    private Coroutine changeAnimationCoroutine;

    void Start()
    {
        InitializeDisplay();
    }

    void InitializeDisplay()
    {
        if (priceText != null)
        {
            priceText.text = "0.00";
        }

        if (changeText != null)
        {
            changeText.text = "";
            changeText.gameObject.SetActive(showChange);
        }
    }

    public void UpdatePrice(float newPrice)
    {
        previousPrice = currentPrice;
        currentPrice = newPrice;

        UpdatePriceText();

        if (showChange && previousPrice != 0f)
        {
            ShowPriceChange();
        }
    }

    void UpdatePriceText()
    {
        if (priceText != null)
        {
            priceText.text = currentPrice.ToString(priceFormat);
        }
    }

    void ShowPriceChange()
    {
        float change = currentPrice - previousPrice;

        if (changeText != null)
        {
            string changeString = "";
            Color changeColor = neutralColor;

            if (change > 0)
            {
                changeString = $"+{change.ToString(priceFormat)}";
                changeColor = positiveChangeColor;
            }
            else if (change < 0)
            {
                changeString = change.ToString(priceFormat);
                changeColor = negativeChangeColor;
            }
            else
            {
                changeString = "0.00";
                changeColor = neutralColor;
            }

            changeText.text = changeString;
            changeText.color = changeColor;

            // 启动变化动画
            if (changeAnimationCoroutine != null)
            {
                StopCoroutine(changeAnimationCoroutine);
            }
            changeAnimationCoroutine = StartCoroutine(AnimatePriceChange(changeColor));
        }
    }

    IEnumerator AnimatePriceChange(Color targetColor)
    {
        if (backgroundImage == null)
            yield break;

        Color startColor = backgroundImage.color;
        float elapsed = 0f;

        while (elapsed < changeAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / changeAnimationDuration;

            backgroundImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        // 恢复原始颜色
        yield return new WaitForSeconds(0.5f);

        elapsed = 0f;
        while (elapsed < changeAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / changeAnimationDuration;

            backgroundImage.color = Color.Lerp(targetColor, Color.white, t);

            yield return null;
        }

        backgroundImage.color = Color.white;
    }

    // 设置价格格式
    public void SetPriceFormat(string format)
    {
        priceFormat = format;
        UpdatePriceText();
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

    // 设置颜色
    public void SetPositiveChangeColor(Color color)
    {
        positiveChangeColor = color;
    }

    public void SetNegativeChangeColor(Color color)
    {
        negativeChangeColor = color;
    }

    // 获取当前价格
    public float GetCurrentPrice()
    {
        return currentPrice;
    }

    // 获取价格变化
    public float GetPriceChange()
    {
        return currentPrice - previousPrice;
    }
}