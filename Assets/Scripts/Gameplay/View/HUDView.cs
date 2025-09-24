using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDView : LevelView
{
    private HUDCom com;
    public GameObject hudComPrefab;
    public string moneyFormat = "N0";
    public bool showChange = true;
    public float changeAnimationDuration = 0.5f;
    [Header("动画设置")]
    public bool enableCountAnimation = true;
    public float countAnimationDuration = 1f;
    private float previousMoney = 0;
    private Coroutine changeAnimationCoroutine;
    private Coroutine countAnimationCoroutine;
    void Awake()
    {
        com = Instantiate(hudComPrefab).GetComponent<HUDCom>();
        AddCanvasCom(com.gameObject);
        com.nextRoundButton.onClick.AddListener(OnNextRoundButtonClick);
    }
    protected override void OnShow()
    {
        Register(NotifyConst.UpdatePlayerAttr, OnUpdatePlayerAttr);
        Register(NotifyConst.UpdateManaUI, OnUpdateMana);
        Register(NotifyConst.UpdateMoneyUI, OnUpdateMoney);
        com.attrViewCom.Init();
        if (com.moneyText != null)
        {
            com.moneyText.text = $"金钱: {0.ToString(moneyFormat)}";
        }

        if (com.changeText != null)
        {
            com.changeText.text = "";
            com.changeText.gameObject.SetActive(showChange);
        }
        previousMoney = Data.money;
    }
    protected override void OnHide()
    {
        Unregister(NotifyConst.UpdatePlayerAttr, OnUpdatePlayerAttr);
        Unregister(NotifyConst.UpdateManaUI, OnUpdateMana);
        Unregister(NotifyConst.UpdateMoneyUI, OnUpdateMoney);

    }
    private void OnUpdatePlayerAttr()
    {
        com.attrViewCom.UpdateAllDisplays();
    }
    private void OnUpdateMana()
    {
        com.mana.text = Data.mana.ToString();
    }
    private void OnUpdateMoney()
    {
        if (enableCountAnimation)
            StartCountAnimation();
        else
            UpdateMoneyText();

        if (showChange && previousMoney != 0)
            ShowMoneyChange();

        previousMoney = Data.money;
    }

    void UpdateMoneyText()
    {
        com.moneyText.text = $"金钱: {Data.money.ToString(moneyFormat)}";
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
        float targetMoney = Data.money;

        while (elapsed < countAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / countAnimationDuration;

            // 使用缓动函数使动画更自然
            t = Mathf.SmoothStep(0f, 1f, t);

            float currentValue = Mathf.RoundToInt(Mathf.Lerp(startMoney, targetMoney, t));

            if (com.moneyText != null)
            {
                com.moneyText.text = $"金钱: {currentValue.ToString(moneyFormat)}";
            }

            yield return null;
        }

        UpdateMoneyText();
    }

    void ShowMoneyChange()
    {
        float change = Data.money - previousMoney;

        if (com.changeText != null)
        {
            string changeString;
            Color changeColor;
            if (change > 0)
            {
                changeString = $"+{change.ToString(moneyFormat)}";
                changeColor = Color.green;
            }
            else if (change < 0)
            {
                changeString = change.ToString(moneyFormat);
                changeColor = Color.red;
            }
            else
            {
                changeString = "0";
                changeColor = Color.white;
            }

            com.changeText.text = changeString;
            com.changeText.color = changeColor;

            // 启动变化动画
            if (changeAnimationCoroutine != null)
            {
                StopCoroutine(changeAnimationCoroutine);
            }
            changeAnimationCoroutine = StartCoroutine(AnimateMoneyChange());
        }
    }

    IEnumerator AnimateMoneyChange()
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
    private void OnNextRoundButtonClick()
    {
        // 检查是否处于冷却状态
        if (GM.Ins.Level.IsInPerform) return;
        AddCMD(new NextRoundTurnCMD());
    }
}