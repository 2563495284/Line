/*
=== 多股市和玩家属性系统使用指南 ===

这个系统实现了完整的多股市交易和玩家属性成长机制，为游戏提供了深度的策略性和成长性。

## 核心功能

### 1. 多股市系统
- **三个独立股市**: 石油、钢铁、棉花
- **独立价格波动**: 每个股市有自己的价格历史和波动性
- **买卖交易**: 支持股票买卖赚取金币
- **材料消耗**: 股票可作为卡牌的消耗材料

### 2. 玩家属性系统
- **社交**: 每回合摸牌数+1
- **耐心**: 保留剩余能量到下回合
- **智慧**: 每回合能量恢复+1
- **魅力**: 股市影响力+10%

### 3. 能量系统
- **基础能量**: 每回合3点能量
- **智慧加成**: 智慧属性增加能量恢复
- **耐心保存**: 耐心属性可保存剩余能量

## 系统架构

### 数据层
```
StockMarketData.cs - 股市数据结构
├── EStockType - 股票类型枚举
├── SingleStockMarketData - 单个股市数据
└── 价格历史、持有量、波动性管理

PlayerAttributeData.cs - 玩家属性数据
├── EPlayerAttributeType - 属性类型枚举
├── PlayerAttributeData - 单个属性数据
└── PlayerAttributesData - 属性系统数据
```

### 系统层
```
MultiStockSystem.cs - 多股市管理系统
├── 价格刷新和波动计算
├── 股票交易处理
├── 材料使用管理
└── UI更新和事件触发

PlayerAttributeSystem.cs - 玩家属性系统
├── 属性升级管理
├── 能量系统管理
├── 回合管理
└── 属性效果应用
```

### GameAction层
```
TradeSpecificStockGA - 特定股票交易
UseStockMaterialGA - 使用股票材料
UpgradeAttributeGA - 升级玩家属性
UseEnergyGA - 使用能量
RestoreEnergyGA - 恢复能量
```

### Effect层
```
TradeStockEffect - 股票交易效果
UseStockMaterialEffect - 材料消耗效果
UseEnergyEffect - 能量消耗效果
```

### UI层
```
MultiStockDisplay - 多股市显示UI
├── StockDisplayItem - 单个股票显示项
└── 实时价格、持有量、交易按钮

PlayerAttributeDisplay - 玩家属性显示UI
├── AttributeDisplayItem - 单个属性显示项
└── 等级、数值、升级按钮
```

## 使用流程

### 1. 系统初始化
1. 在场景中添加MultiStockSystem组件
2. 在场景中添加PlayerAttributeSystem组件
3. 配置UI组件和Prefab引用

### 2. 股票交易
```csharp
// 买入10股石油
var buyGA = new TradeSpecificStockGA(EStockType.Oil, 10);
ActionSystem.Instance.Perform(buyGA);

// 卖出5股钢铁
var sellGA = new TradeSpecificStockGA(EStockType.Steel, -5);
ActionSystem.Instance.Perform(sellGA);
```

### 3. 材料消耗
```csharp
// 在卡牌Effect中使用材料
public class ExampleCardEffect : Effect
{
    [SerializeField] private UseStockMaterialEffect materialEffect;
    
    public override GameAction GetGameAction()
    {
        // 检查材料是否足够
        if (materialEffect.CanUse())
        {
            return materialEffect.GetGameAction();
        }
        return null;
    }
}
```

### 4. 属性升级
```csharp
// 升级社交属性
var upgradeGA = new UpgradeAttributeGA(EPlayerAttributeType.Social);
ActionSystem.Instance.Perform(upgradeGA);
```

### 5. 回合管理
```csharp
// 开始新回合
PlayerAttributeSystem.Instance.StartNewTurn();

// 使用能量
var useEnergyGA = new UseEnergyGA(2);
ActionSystem.Instance.Perform(useEnergyGA);
```

## 属性效果详解

### 社交属性
- **基础效果**: 每级+1摸牌数
- **应用时机**: 回合开始时
- **计算公式**: 摸牌数 = 基础摸牌数 + 社交等级

### 耐心属性
- **基础效果**: 每级可保存1点能量
- **应用时机**: 回合结束时
- **计算公式**: 保存能量 = min(剩余能量, 耐心等级)

### 智慧属性
- **基础效果**: 每级+1能量恢复
- **应用时机**: 回合开始时
- **计算公式**: 能量恢复 = 基础恢复 + 智慧等级

### 魅力属性
- **基础效果**: 每级+10%股市影响力
- **应用时机**: 股价波动时
- **计算公式**: 影响倍数 = 1 + (魅力等级 × 10%)

## 扩展机制

### 1. 添加新属性
```csharp
// 在EPlayerAttributeType中添加新枚举
public enum EPlayerAttributeType
{
    Social, Patience, Wisdom, Charisma,
    NewAttribute // 新属性
}

// 在PlayerAttributeData.SetupAttributeInfo()中添加配置
case EPlayerAttributeType.NewAttribute:
    attributeName = "新属性";
    description = "新属性效果描述";
    baseValue = 0f;
    valuePerLevel = 1f;
    upgradeCost = 100;
    break;
```

### 2. 添加新股票类型
```csharp
// 在EStockType中添加新枚举
public enum EStockType
{
    Oil, Steel, Cotton,
    NewStock // 新股票
}

// 在SingleStockMarketData.SetupStockInfo()中添加配置
case EStockType.NewStock:
    stockName = "新股票";
    stockSymbol = "NEW";
    themeColor = Color.blue;
    baseVolatility = 1.0f;
    break;
```

### 3. 添加新效果类型
```csharp
// 创建新的Effect类
public class NewAttributeEffect : Effect
{
    // 实现新的属性效果逻辑
}

// 创建对应的GameAction
public class NewAttributeGA : GameAction
{
    // 实现新的游戏行为
}
```

## 平衡性考虑

### 1. 属性升级成本
- **指数增长**: 每级费用按1.5倍递增
- **差异化定价**: 不同属性有不同基础费用
- **上限控制**: 每个属性最高10级

### 2. 股市波动性
- **差异化波动**: 石油>棉花>钢铁
- **魅力影响**: 魅力属性放大波动（正负都放大）
- **价格限制**: 每个股市有最低和最高价格

### 3. 能量平衡
- **基础能量**: 每回合3点
- **使用限制**: 大部分卡牌消耗1-2点能量
- **保存机制**: 通过耐心属性实现能量积累

## 调试功能

### 系统状态查看
- MultiStockSystem.PrintAllStockStatus() - 打印股市状态
- PlayerAttributeSystem.PrintAttributeStatus() - 打印属性状态
- 各种Context Menu测试功能

### 快速测试
- MultiStockAndAttributeExample.cs - 完整功能演示
- 自动测试模式和手动测试方法
- 系统集成测试

这个系统为游戏提供了丰富的策略深度和成长机制，玩家可以通过合理的资源配置和属性投资来优化自己的游戏体验。
*/

using UnityEngine;

/// <summary>
/// 多股市和玩家属性系统使用指南
/// </summary>
public class MultiStockAndAttributeGuide : MonoBehaviour
{
    [Header("使用指南")]
    [TextArea(10, 20)]
    [SerializeField] private string guide = "请查看此脚本的源代码注释获取详细使用指南";

    private void Start()
    {
        Debug.Log("多股市和玩家属性系统指南已加载。查看MultiStockAndAttributeGuide脚本获取详细说明。");
    }

    /// <summary>
    /// 验证系统设置
    /// </summary>
    [ContextMenu("验证系统设置")]
    public void ValidateSystemSetup()
    {
        Debug.Log("=== 系统设置验证 ===");

        // 检查MultiStockSystem
        if (MultiStockSystem.Instance == null)
        {
            Debug.LogError("未找到MultiStockSystem实例！");
        }
        else
        {
            Debug.Log("✓ MultiStockSystem已找到");
        }

        // 检查PlayerAttributeSystem
        if (PlayerAttributeSystem.Instance == null)
        {
            Debug.LogError("未找到PlayerAttributeSystem实例！");
        }
        else
        {
            Debug.Log("✓ PlayerAttributeSystem已找到");
        }

        // 检查BuffSystem
        if (BuffSystem.Instance == null)
        {
            Debug.LogWarning("未找到BuffSystem实例（可选）");
        }
        else
        {
            Debug.Log("✓ BuffSystem已找到");
        }

        Debug.Log("验证完成");
    }
}
