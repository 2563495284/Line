using UnityEngine;
using System.Collections;

/// <summary>
/// 多股市和玩家属性系统使用示例
/// </summary>
public class MultiStockAndAttributeExample : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool autoTest = false;
    [SerializeField] private float testInterval = 3f;

    private void Start()
    {
        Debug.Log("多股市和玩家属性系统示例已加载");

        if (autoTest)
        {
            StartCoroutine(AutoTestCoroutine());
        }
    }

    /// <summary>
    /// 自动测试协程
    /// </summary>
    private IEnumerator AutoTestCoroutine()
    {
        yield return new WaitForSeconds(2f); // 等待系统初始化

        while (true)
        {
            TestStockOperations();
            yield return new WaitForSeconds(testInterval);

            TestAttributeUpgrade();
            yield return new WaitForSeconds(testInterval);

            TestMaterialUsage();
            yield return new WaitForSeconds(testInterval);

            TestTurnSystem();
            yield return new WaitForSeconds(testInterval);
        }
    }

    #region Stock System Tests

    /// <summary>
    /// 测试股票操作
    /// </summary>
    [ContextMenu("测试股票操作")]
    public void TestStockOperations()
    {
        Debug.Log("=== 测试股票操作 ===");

        // 买入石油
        var buyOilGA = new TradeSpecificStockGA(EStockType.Oil, 5);
        ActionSystem.Instance.Perform(buyOilGA);

        // 买入钢铁
        var buySteelGA = new TradeSpecificStockGA(EStockType.Steel, 3);
        ActionSystem.Instance.Perform(buySteelGA);

        // 买入棉花
        var buyCottonGA = new TradeSpecificStockGA(EStockType.Cotton, 2);
        ActionSystem.Instance.Perform(buyCottonGA);

        Debug.Log("已执行股票买入操作");
    }

    /// <summary>
    /// 测试卖出股票
    /// </summary>
    [ContextMenu("测试卖出股票")]
    public void TestSellStocks()
    {
        Debug.Log("=== 测试卖出股票 ===");

        // 卖出部分石油
        var sellOilGA = new TradeSpecificStockGA(EStockType.Oil, -2);
        ActionSystem.Instance.Perform(sellOilGA);

        Debug.Log("已执行股票卖出操作");
    }

    /// <summary>
    /// 测试材料使用
    /// </summary>
    [ContextMenu("测试材料使用")]
    public void TestMaterialUsage()
    {
        Debug.Log("=== 测试材料使用 ===");

        // 使用石油材料
        var useOilGA = new UseStockMaterialGA(EStockType.Oil, 1);
        ActionSystem.Instance.Perform(useOilGA);

        // 使用钢铁材料
        var useSteelGA = new UseStockMaterialGA(EStockType.Steel, 1);
        ActionSystem.Instance.Perform(useSteelGA);

        Debug.Log("已执行材料使用操作");
    }

    #endregion

    #region Attribute System Tests

    /// <summary>
    /// 测试属性升级
    /// </summary>
    [ContextMenu("测试属性升级")]
    public void TestAttributeUpgrade()
    {
        Debug.Log("=== 测试属性升级 ===");

        // 升级社交属性
        var upgradeSocialGA = new UpgradeAttributeGA(EPlayerAttributeType.Social);
        ActionSystem.Instance.Perform(upgradeSocialGA);

        Debug.Log("已尝试升级社交属性");
    }

    /// <summary>
    /// 测试升级所有属性
    /// </summary>
    [ContextMenu("测试升级所有属性")]
    public void TestUpgradeAllAttributes()
    {
        Debug.Log("=== 测试升级所有属性 ===");

        // 尝试升级每个属性
        foreach (EPlayerAttributeType attributeType in System.Enum.GetValues(typeof(EPlayerAttributeType)))
        {
            var upgradeGA = new UpgradeAttributeGA(attributeType);
            ActionSystem.Instance.Perform(upgradeGA);
        }

        Debug.Log("已尝试升级所有属性");
    }

    /// <summary>
    /// 测试回合系统
    /// </summary>
    [ContextMenu("测试回合系统")]
    public void TestTurnSystem()
    {
        Debug.Log("=== 测试回合系统 ===");

        if (PlayerAttributeSystem.Instance != null)
        {
            // 使用一些能量
            var useEnergyGA = new UseEnergyGA(2);
            ActionSystem.Instance.Perform(useEnergyGA);

            // 开始新回合
            PlayerAttributeSystem.Instance.StartNewTurn();
        }

        Debug.Log("已执行回合系统测试");
    }

    #endregion

    #region Energy System Tests

    /// <summary>
    /// 测试能量系统
    /// </summary>
    [ContextMenu("测试能量系统")]
    public void TestEnergySystem()
    {
        Debug.Log("=== 测试能量系统 ===");

        // 使用能量
        var useEnergyGA = new UseEnergyGA(1);
        ActionSystem.Instance.Perform(useEnergyGA);

        // 恢复能量
        var restoreEnergyGA = new RestoreEnergyGA(2);
        ActionSystem.Instance.Perform(restoreEnergyGA);

        Debug.Log("已执行能量系统测试");
    }

    #endregion

    #region Status Display

    /// <summary>
    /// 打印系统状态
    /// </summary>
    [ContextMenu("打印系统状态")]
    public void PrintSystemStatus()
    {
        Debug.Log("=== 系统状态总览 ===");

        // 打印股市状态
        if (MultiStockSystem.Instance != null)
        {
            MultiStockSystem.Instance.PrintAllStockStatus();
        }

        // 打印属性状态
        if (PlayerAttributeSystem.Instance != null)
        {
            PlayerAttributeSystem.Instance.PrintAttributeStatus();
        }

        // 打印Buff状态
        if (BuffSystem.Instance != null)
        {
            BuffSystem.Instance.PrintBuffStatus();
        }
    }

    /// <summary>
    /// 测试完整游戏流程
    /// </summary>
    [ContextMenu("测试完整游戏流程")]
    public void TestCompleteGameFlow()
    {
        StartCoroutine(CompleteGameFlowCoroutine());
    }

    /// <summary>
    /// 完整游戏流程测试协程
    /// </summary>
    private IEnumerator CompleteGameFlowCoroutine()
    {
        Debug.Log("=== 开始完整游戏流程测试 ===");

        // 1. 初始状态
        Debug.Log("1. 打印初始状态");
        PrintSystemStatus();
        yield return new WaitForSeconds(2f);

        // 2. 买入一些股票
        Debug.Log("2. 买入股票建立资产");
        TestStockOperations();
        yield return new WaitForSeconds(2f);

        // 3. 升级属性
        Debug.Log("3. 升级玩家属性");
        TestAttributeUpgrade();
        yield return new WaitForSeconds(2f);

        // 4. 添加杠杆Buff
        Debug.Log("4. 添加杠杆Buff");
        if (BuffSystem.Instance != null)
        {
            BuffSystem.Instance.AddTestLeverageBuff();
        }
        yield return new WaitForSeconds(2f);

        // 5. 使用材料
        Debug.Log("5. 使用股票作为材料");
        TestMaterialUsage();
        yield return new WaitForSeconds(2f);

        // 6. 模拟几个回合
        Debug.Log("6. 模拟多个回合");
        for (int i = 1; i <= 3; i++)
        {
            Debug.Log($"--- 第{i}回合 ---");
            TestTurnSystem();
            yield return new WaitForSeconds(2f);
        }

        // 7. 最终状态
        Debug.Log("7. 打印最终状态");
        PrintSystemStatus();

        Debug.Log("=== 完整游戏流程测试结束 ===");
    }

    #endregion

    #region Integration Tests

    /// <summary>
    /// 测试系统集成
    /// </summary>
    [ContextMenu("测试系统集成")]
    public void TestSystemIntegration()
    {
        Debug.Log("=== 测试系统集成 ===");

        // 测试魅力属性对股市的影响
        TestCharismaStockInfluence();

        // 测试耐心属性的能量保存
        TestPatienceEnergySystem();

        // 测试社交属性的摸牌加成
        TestSocialCardBonus();

        Debug.Log("系统集成测试完成");
    }

    /// <summary>
    /// 测试魅力属性对股市的影响
    /// </summary>
    private void TestCharismaStockInfluence()
    {
        Debug.Log("测试魅力属性对股市影响...");

        if (PlayerAttributeSystem.Instance != null)
        {
            float charismaBonus = PlayerAttributeSystem.Instance.GetStockInfluenceBonus();
            Debug.Log($"当前魅力加成: +{charismaBonus * 100:F1}%");
        }
    }

    /// <summary>
    /// 测试耐心属性的能量保存
    /// </summary>
    private void TestPatienceEnergySystem()
    {
        Debug.Log("测试耐心属性能量保存...");

        if (PlayerAttributeSystem.Instance != null)
        {
            int saveableEnergy = PlayerAttributeSystem.Instance.GetSaveableEnergy();
            Debug.Log($"可保存能量: {saveableEnergy}");
        }
    }

    /// <summary>
    /// 测试社交属性的摸牌加成
    /// </summary>
    private void TestSocialCardBonus()
    {
        Debug.Log("测试社交属性摸牌加成...");

        if (PlayerAttributeSystem.Instance != null)
        {
            int cardsPerTurn = PlayerAttributeSystem.Instance.GetCardsPerTurn();
            Debug.Log($"每回合摸牌数: {cardsPerTurn}");
        }
    }

    #endregion
}
