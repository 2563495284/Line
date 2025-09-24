using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class StockChartCom : MonoBehaviour
{
    public EStockType stockType;
    [Header("UI组件")]
    public TextMeshPro stockText;
    public TextMeshPro changeText;
    public TextMeshPro StockPrice;


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

    public void Init()
    {
        UpdateStockText();
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
        StockPrice.text = "当前价格: " + MultiStockSystem.Ins.GetStockMarket(stockType).currentPrice;

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
            switch (stockType)
            {
                case EStockType.Oil:
                    stockText.text = $"拥有石油数量: {currentStock.ToString(stockFormat)}";
                    break;
                case EStockType.Cotton:
                    stockText.text = $"拥有棉花数量: {currentStock.ToString(stockFormat)}";
                    break;
                case EStockType.Steel:
                    stockText.text = $"拥有钢铁数量: {currentStock.ToString(stockFormat)}";
                    break;
            }
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
                switch (stockType)
                {
                    case EStockType.Oil:
                        stockText.text = $"拥有石油数量: {currentValue.ToString(stockFormat)}";
                        break;
                    case EStockType.Cotton:
                        stockText.text = $"拥有棉花数量: {currentValue.ToString(stockFormat)}";
                        break;
                    case EStockType.Steel:
                        stockText.text = $"拥有钢铁数量: {currentValue.ToString(stockFormat)}";
                        break;
                }
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

}