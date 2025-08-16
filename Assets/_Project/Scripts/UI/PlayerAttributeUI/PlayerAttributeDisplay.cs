using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 玩家属性显示UI
/// </summary>
public class PlayerAttributeDisplay : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Transform attributeItemContainer;
    [SerializeField] private GameObject attributeItemPrefab;

    [Header("状态显示")]
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI cardsPerTurnText;
    [SerializeField] private TextMeshProUGUI totalPointsText;

    [Header("更新设置")]
    [SerializeField] private float updateInterval = 1f;

    private List<AttributeDisplayItem> attributeDisplayItems = new List<AttributeDisplayItem>();
    private Coroutine updateCoroutine;

    private void Start()
    {
        InitializeAttributeDisplay();
        StartUpdateCoroutine();
    }

    private void OnDestroy()
    {
        StopUpdateCoroutine();
    }

    #region Initialization

    /// <summary>
    /// 初始化属性显示
    /// </summary>
    private void InitializeAttributeDisplay()
    {
        if (PlayerAttributeSystem.Instance == null)
        {
            Debug.LogError("PlayerAttributeSystem未找到！");
            return;
        }

        var playerAttributes = PlayerAttributeSystem.Instance.GetPlayerAttributes();

        foreach (var attribute in playerAttributes.attributes)
        {
            CreateAttributeDisplayItem(attribute);
        }
    }

    /// <summary>
    /// 创建属性显示项
    /// </summary>
    private void CreateAttributeDisplayItem(PlayerAttributeData attributeData)
    {
        if (attributeItemPrefab == null || attributeItemContainer == null)
        {
            Debug.LogError("缺少必要的UI组件引用！");
            return;
        }

        GameObject itemObj = Instantiate(attributeItemPrefab, attributeItemContainer);
        AttributeDisplayItem displayItem = itemObj.GetComponent<AttributeDisplayItem>();

        if (displayItem != null)
        {
            displayItem.Initialize(attributeData);
            attributeDisplayItems.Add(displayItem);
        }
        else
        {
            Debug.LogError("AttributeItemPrefab缺少AttributeDisplayItem组件！");
            Destroy(itemObj);
        }
    }

    #endregion

    #region Update System

    /// <summary>
    /// 开始更新协程
    /// </summary>
    private void StartUpdateCoroutine()
    {
        if (updateCoroutine == null)
        {
            updateCoroutine = StartCoroutine(UpdateDisplayCoroutine());
        }
    }

    /// <summary>
    /// 停止更新协程
    /// </summary>
    private void StopUpdateCoroutine()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
    }

    /// <summary>
    /// 更新显示协程
    /// </summary>
    private IEnumerator UpdateDisplayCoroutine()
    {
        while (true)
        {
            UpdateAllDisplays();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    /// <summary>
    /// 更新所有显示
    /// </summary>
    public void UpdateAllDisplays()
    {
        if (PlayerAttributeSystem.Instance == null) return;

        // 更新各个属性显示
        foreach (var displayItem in attributeDisplayItems)
        {
            displayItem.UpdateDisplay();
        }

        // 更新状态显示
        UpdateStatusDisplay();
    }

    /// <summary>
    /// 更新状态显示
    /// </summary>
    private void UpdateStatusDisplay()
    {
        if (PlayerAttributeSystem.Instance == null) return;

        // 更新能量显示
        if (energyText != null)
        {
            int currentEnergy = PlayerAttributeSystem.Instance.GetCurrentEnergy();
            int energyPerTurn = PlayerAttributeSystem.Instance.GetEnergyPerTurn();
            energyText.text = $"能量: {currentEnergy} (每回合+{energyPerTurn})";
        }

        // 更新摸牌数显示
        if (cardsPerTurnText != null)
        {
            int cardsPerTurn = PlayerAttributeSystem.Instance.GetCardsPerTurn();
            cardsPerTurnText.text = $"每回合摸牌: {cardsPerTurn}";
        }

        // 更新总属性点显示
        if (totalPointsText != null)
        {
            var playerAttributes = PlayerAttributeSystem.Instance.GetPlayerAttributes();
            totalPointsText.text = $"总属性点: {playerAttributes.totalAttributePoints}";
        }
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// 手动刷新显示
    /// </summary>
    [ContextMenu("刷新显示")]
    public void RefreshDisplay()
    {
        UpdateAllDisplays();
    }

    /// <summary>
    /// 开始新回合
    /// </summary>
    [ContextMenu("开始新回合")]
    public void StartNewTurn()
    {
        if (PlayerAttributeSystem.Instance != null)
        {
            PlayerAttributeSystem.Instance.StartNewTurn();
        }
    }

    #endregion
}
