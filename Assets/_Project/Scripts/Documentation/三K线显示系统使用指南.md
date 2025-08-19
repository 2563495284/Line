# 三K线显示系统使用指南

## 概述

三K线显示系统是一个专为多股市游戏设计的实时股价图表显示组件。它能够在屏幕中央并排显示三种股票（石油、钢铁、棉花）的K线图，提供直观的价格走势可视化。

## 主要组件

### 1. TripleKLineDisplay（三K线显示管理器）
- **功能**：管理三个并排的K线图显示
- **特性**：
  - 自动布局三个LineView组件
  - 实时数据更新
  - 事件驱动的价格变化响应
  - 可配置的更新间隔和布局参数

### 2. 优化版LineView（K线图显示组件）
- **新增功能**：
  - 股票信息显示（标题、当前价格、变化百分比）
  - 主题颜色支持
  - 更好的价格标签显示
  - 实时价格更新方法

### 3. TripleKLineCreator（创建器工具）
- **功能**：快速创建和配置三K线显示系统
- **使用场景**：
  - 从零创建完整的三K线系统
  - 组装现有的LineView组件

## 使用步骤

### 方法1：使用创建器工具（推荐）

1. **准备预制体**：
   ```
   - LineView预制体（确保包含LineRenderer、背景等组件）
   - TextMeshPro文本预制体（可选）
   ```

2. **添加创建器组件**：
   ```csharp
   // 在场景中创建空对象，添加TripleKLineCreator组件
   GameObject creator = new GameObject("KLineCreator");
   TripleKLineCreator creatorComponent = creator.AddComponent<TripleKLineCreator>();
   ```

3. **配置创建器参数**：
   - `lineViewPrefab`：设置LineView预制体
   - `textPrefab`：设置文本预制体（可选）
   - `centerPosition`：设置中心位置
   - `spacing`：设置K线图间距（推荐15f）
   - `chartWidth/chartHeight`：设置图表尺寸

4. **执行创建**：
   - 在Inspector中点击"创建三K线显示系统"
   - 或通过代码调用：`creatorComponent.CreateTripleKLineSystem()`

### 方法2：手动配置

1. **创建TripleKLineDisplay对象**：
   ```csharp
   GameObject container = new GameObject("TripleKLineDisplay");
   TripleKLineDisplay tripleDisplay = container.AddComponent<TripleKLineDisplay>();
   ```

2. **创建三个LineView**：
   ```csharp
   // 创建石油K线
   GameObject oilObj = Instantiate(lineViewPrefab);
   LineView oilView = oilObj.GetComponent<LineView>();
   oilView.SetStockInfo(EStockType.Oil, "石油", new Color(0.2f, 0.2f, 0.2f));
   
   // 创建钢铁K线
   GameObject steelObj = Instantiate(lineViewPrefab);
   LineView steelView = steelObj.GetComponent<LineView>();
   steelView.SetStockInfo(EStockType.Steel, "钢铁", new Color(0.7f, 0.7f, 0.7f));
   
   // 创建棉花K线
   GameObject cottonObj = Instantiate(lineViewPrefab);
   LineView cottonView = cottonObj.GetComponent<LineView>();
   cottonView.SetStockInfo(EStockType.Cotton, "棉花", new Color(0.9f, 0.9f, 0.8f));
   ```

3. **设置引用**：
   ```csharp
   tripleDisplay.SetLineViewReferences(oilView, steelView, cottonView);
   ```

### 方法3：组装现有LineView

如果场景中已有LineView组件：

1. **添加创建器**：
   ```csharp
   TripleKLineCreator creator = gameObject.AddComponent<TripleKLineCreator>();
   ```

2. **执行组装**：
   - 调用`creator.AssembleExistingLineViews()`
   - 手动在TripleKLineDisplay中设置LineView引用

## 与MultiStockSystem集成

### 1. 在MultiStockSystem中添加引用

```csharp
[Header("UI引用")]
[SerializeField] private TripleKLineDisplay tripleKLineDisplay;
```

### 2. 确保数据更新

系统已自动集成，MultiStockSystem会：
- 定期刷新股价时更新K线显示
- 响应股价变化事件
- 提供实时数据同步

## 配置参数说明

### TripleKLineDisplay参数

- **spacing**：K线图之间的间距（推荐：15f）
- **centerPosition**：整个系统的中心位置
- **updateInterval**：自动更新间隔（秒）
- **autoUpdate**：是否启用自动更新

### LineView新增参数

- **titleText**：标题文本组件引用
- **currentPriceText**：当前价格文本组件引用
- **changePercentText**：变化百分比文本组件引用
- **positiveColor/negativeColor/neutralColor**：涨跌颜色配置

## 事件系统

系统会自动响应以下事件：
- `MultiStockSystem.OnStockPriceChanged`：股价变化时更新对应K线
- 定时器事件：定期刷新所有显示

## 调试功能

### 开发时调试

1. **测试随机数据**：
   ```csharp
   tripleDisplay.TestAddRandomData(); // 添加随机价格数据
   ```

2. **手动刷新**：
   ```csharp
   tripleDisplay.RefreshAllDisplays(); // 手动刷新所有显示
   ```

3. **打印状态**：
   ```csharp
   tripleDisplay.PrintCurrentStatus(); // 打印当前配置状态
   ```

### Inspector调试选项

- 启用`showDebugInfo`查看详细日志
- 使用Context Menu快捷操作

## 性能优化建议

1. **合理设置更新频率**：
   - 一般游戏：1-3秒更新一次
   - 高频交易游戏：0.5-1秒更新一次

2. **限制历史数据点数**：
   - 推荐maxPoints设置为40-60
   - 避免过多历史数据影响性能

3. **优化渲染**：
   - 使用LOD系统处理远距离显示
   - 考虑使用对象池管理文本标签

## 常见问题解决

### Q: K线图显示位置不正确
A: 检查centerPosition和spacing设置，确保有足够的显示空间

### Q: 股票信息不显示
A: 确保已正确设置TextMeshPro组件引用，并调用SetStockInfo方法

### Q: 数据不更新
A: 检查MultiStockSystem的tripleKLineDisplay引用是否正确设置

### Q: 性能问题
A: 降低更新频率，减少maxPoints数量，检查是否有内存泄漏

## 扩展开发

### 添加新股票类型

1. 在EStockType枚举中添加新类型
2. 更新SingleStockMarketData的SetupStockInfo方法
3. 修改TripleKLineDisplay以支持更多K线图

### 自定义样式

1. 继承LineView类实现自定义渲染
2. 修改TripleKLineDisplay的布局算法
3. 添加新的主题颜色和样式配置

### 数据持久化

1. 实现价格历史数据的保存/加载
2. 添加用户偏好设置保存
3. 支持导出图表数据

## 版本信息

- **当前版本**：1.0.0
- **兼容Unity版本**：2021.3+
- **依赖组件**：TextMeshPro, DOTween
- **创建日期**：2024年

---

*如需更多帮助，请查看示例场景或联系开发团队。*
