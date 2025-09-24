using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Buff UI管理器 - 管理所有Buff的UI显示
/// </summary>
public class BuffUIManager : SingletonCom<BuffUIManager>
{
    [Header("UI容器")]
    [SerializeField] private Transform buffContainer;
    [SerializeField] private GameObject buffItemPrefab;
    [SerializeField] private LayoutGroup layoutGroup;

    [Header("显示设置")]
    [SerializeField] private int maxDisplayItems = 10;
    [SerializeField] private bool autoHideWhenEmpty = true;
    [SerializeField] private float updateInterval = 0.5f;

    [Header("动画设置")]
    [SerializeField] private float containerFadeInDuration = 0.3f;
    [SerializeField] private float containerFadeOutDuration = 0.5f;

    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;

    private List<BuffDisplayItem> buffDisplayItems = new List<BuffDisplayItem>();
    private Dictionary<string, BuffDisplayItem> activeBuffItems = new Dictionary<string, BuffDisplayItem>();
    private Coroutine updateCoroutine;
    private CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();
        InitializeUI();
    }

    private void Start()
    {
        StartUpdateCoroutine();
    }

    private void OnDestroy()
    {
        StopUpdateCoroutine();
    }

    #region Initialization

    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        // 获取或添加CanvasGroup组件
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 初始化时隐藏容器
        if (autoHideWhenEmpty)
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        // 预创建Buff显示项
        CreateBuffDisplayItems();
    }

    /// <summary>
    /// 创建Buff显示项
    /// </summary>
    private void CreateBuffDisplayItems()
    {
        if (buffItemPrefab == null || buffContainer == null)
        {
            Debug.LogError("BuffUIManager: 缺少必要的Prefab或Container引用!");
            return;
        }

        for (int i = 0; i < maxDisplayItems; i++)
        {
            GameObject itemObj = Instantiate(buffItemPrefab, buffContainer);
            BuffDisplayItem displayItem = itemObj.GetComponent<BuffDisplayItem>();

            if (displayItem != null)
            {
                buffDisplayItems.Add(displayItem);
                displayItem.ResetItem();
            }
            else
            {
                Debug.LogError("BuffUIManager: BuffItemPrefab缺少BuffDisplayItem组件!");
                Destroy(itemObj);
            }
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
            updateCoroutine = StartCoroutine(UpdateBuffDisplayCoroutine());
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
    /// 更新Buff显示协程
    /// </summary>
    private IEnumerator UpdateBuffDisplayCoroutine()
    {
        while (true)
        {
            UpdateAllBuffDisplays();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    /// <summary>
    /// 更新所有Buff显示
    /// </summary>
    private void UpdateAllBuffDisplays()
    {
        List<BuffUIData> activeBuffs = CollectActiveBuffs();

        // 更新显示
        UpdateBuffItems(activeBuffs);

        // 控制容器可见性
        UpdateContainerVisibility(activeBuffs.Count > 0);

        if (showDebugInfo)
        {
            Debug.Log($"BuffUIManager: 更新了 {activeBuffs.Count} 个Buff显示");
        }
    }

    /// <summary>
    /// 收集所有活跃的Buff数据
    /// </summary>
    private List<BuffUIData> CollectActiveBuffs()
    {
        List<BuffUIData> buffs = new List<BuffUIData>();

        // 收集杠杆Buff
        if (BuffSystem.Ins != null && BuffSystem.Ins.HasLeverageBuff)
        {
            BuffUIData leverageData = BuffUIData.FromLeverageBuff(BuffSystem.Ins.LeverageBuff);
            if (leverageData.isActive)
            {
                buffs.Add(leverageData);
            }
        }

        // 这里可以添加其他类型的Buff
        // 例如：收集其他Buff系统的数据

        return buffs;
    }

    /// <summary>
    /// 更新Buff显示项
    /// </summary>
    private void UpdateBuffItems(List<BuffUIData> activeBuffs)
    {
        // 重置所有显示项
        Dictionary<string, BuffDisplayItem> newActiveItems = new Dictionary<string, BuffDisplayItem>();

        // 为每个活跃Buff分配显示项
        for (int i = 0; i < activeBuffs.Count && i < buffDisplayItems.Count; i++)
        {
            BuffUIData buffData = activeBuffs[i];
            BuffDisplayItem displayItem = buffDisplayItems[i];

            displayItem.UpdateBuff(buffData);
            newActiveItems[buffData.buffName] = displayItem;
        }

        // 隐藏未使用的显示项
        for (int i = activeBuffs.Count; i < buffDisplayItems.Count; i++)
        {
            buffDisplayItems[i].ResetItem();
        }

        activeBuffItems = newActiveItems;
    }

    /// <summary>
    /// 更新容器可见性
    /// </summary>
    private void UpdateContainerVisibility(bool shouldShow)
    {
        if (!autoHideWhenEmpty) return;

        bool isCurrentlyVisible = gameObject.activeInHierarchy && canvasGroup.alpha > 0.5f;

        if (shouldShow && !isCurrentlyVisible)
        {
            ShowContainer();
        }
        else if (!shouldShow && isCurrentlyVisible)
        {
            HideContainer();
        }
    }

    #endregion

    #region Container Animation

    /// <summary>
    /// 显示容器
    /// </summary>
    private void ShowContainer()
    {
        gameObject.SetActive(true);
        canvasGroup.DOFade(1f, containerFadeInDuration).SetEase(Ease.OutQuad);

        if (showDebugInfo)
        {
            Debug.Log("BuffUIManager: 显示Buff容器");
        }
    }

    /// <summary>
    /// 隐藏容器
    /// </summary>
    private void HideContainer()
    {
        canvasGroup.DOFade(0f, containerFadeOutDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => gameObject.SetActive(false));

        if (showDebugInfo)
        {
            Debug.Log("BuffUIManager: 隐藏Buff容器");
        }
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// 手动刷新显示
    /// </summary>
    [ContextMenu("刷新Buff显示")]
    public void RefreshDisplay()
    {
        UpdateAllBuffDisplays();
    }

    /// <summary>
    /// 获取指定Buff的显示项
    /// </summary>
    private BuffDisplayItem GetBuffDisplayItem(string buffName)
    {
        activeBuffItems.TryGetValue(buffName, out BuffDisplayItem item);
        return item;
    }


    /// <summary>
    /// 播放Buff失效动画
    /// </summary>
    public void PlayBuffExpiredAnimation(string buffName)
    {
        BuffDisplayItem item = GetBuffDisplayItem(buffName);
        if (item != null)
        {
            item.PlayDisappearAnimation();

            if (showDebugInfo)
            {
                Debug.Log($"BuffUIManager: 播放Buff失效动画 - {buffName}");
            }
        }
    }


    /// <summary>
    /// 获取当前显示的Buff数量
    /// </summary>
    public int GetActiveBuffCount()
    {
        return activeBuffItems.Count;
    }

    #endregion

}
