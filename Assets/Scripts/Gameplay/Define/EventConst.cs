using System.Collections.Generic;

public static class EventConst
{
    public static readonly string
        EnterLevelView = "EnterLevelView",
        EnterRouteView = "EnterRouteView",
        EnterShopView = "EnterShopView",
        EnterRoundView = "EnterRoundView",
        UpdatePlayerAttr = "UpdatePlayerAttr",
        UpdateMoneyUI = "UpdateMoneyUI",
        /// <summary> 参数：ShowSidenoteArgs </summary>
        ShowSidenote = "ShowSidenote",
        HideSidenote = "HideSidenote",
        PopupTips = "PopupTips",
        GameLose = "GameLose",
        GameWin = "GameWin",
        DrawActionCards = "DrawActionCards",
        DrawTradeCards = "DrawTradeCards",
        RefillCards = "RefillCards",
        DiscardActionCards = "DiscardActionCards",
        DiscardTradeCards = "DiscardTradeCards",
        CardEffectStart = "CardEffectStart",

        //TODO
        StockStrategyChange = "StockStrategyChange",
        StartStockAttrEffect = "StartStockAttrEffect",
        ExitStockAttrEffect = "ExitStockAttrEffect",
        AddExtraStockAttr = "AddExtraStockAttr",
        RemoveExtraStockAttr = "RemoveExtraStockAttr",
        UpdateStockView = "UpdateStockView",
        StockPerformanceAdded = "StockPerformanceAdded",
        StockPerformanceMuled = "StockPerformanceMuled",
        StockAttrGrown = "StockAttrGrown",
        TransferStockFactor = "TransferStockFactor",
        EnterActionPhase = "EnterActionPhase",
        ExitActionPhase = "ExitActionPhase",
        EnterTradePhase = "EnterTradePhase",
        ExitTradePhase = "ExitTradePhase";
}
public class TransferStockFactorArgs
{
    public int stockId;
    public List<float> oldFactors;
    public List<float> newFactors;
    public TransferStockFactorArgs(int stockId, List<float> oldFactors, List<float> newFactors)
    {
        this.stockId = stockId;
        this.oldFactors = oldFactors;
        this.newFactors = newFactors;
    }
}
public class StockAttrGrownArgs
{
    public int stockId;
    public int attrId;
    public StockAttrGrownArgs(int stockId, int attrId)
    {
        this.stockId = stockId;
        this.attrId = attrId;
    }
}

public class StockPerformanceMuledArgs
{
    public int stockId;
    public float mul;
    public StockPerformanceMuledArgs(int stockId, float mul)
    {
        this.stockId = stockId;
        this.mul = mul;
    }
}
public class RemoveExtraStockAttrArgs
{
    public int stockId;
    public List<int> attrIds;
    public RemoveExtraStockAttrArgs(int stockId, List<int> attrIds)
    {
        this.stockId = stockId;
        this.attrIds = attrIds;
    }
}
public class StockPerformaceAddedArgs
{
    public int stockId;
    public float addVal;
    public StockPerformaceAddedArgs(int stockId, float addVal)
    {
        this.stockId = stockId;
        this.addVal = addVal;
    }
}
public class AddExtraStockAttrArgs
{
    public int stockId;
    public int attrId;
    public AddExtraStockAttrArgs(int stockId, int attrId)
    {
        this.stockId = stockId;
        this.attrId = attrId;
    }
}
public class StartStockAttrEffectArgs
{
    public int stockId;
    public int attrId;
    public StartStockAttrEffectArgs(int stockId, int attrId)
    {
        this.stockId = stockId;
        this.attrId = attrId;
    }
}
public class ExitStockAttrEffectArgs
{
    public int stockId;
    public int attrId;
    public ExitStockAttrEffectArgs(int stockId, int attrId)
    {
        this.stockId = stockId;
        this.attrId = attrId;
    }
}
public class StockStrategyChangeArgs
{
    public int stockId;
    public int strategyId = 0;
    public StockStrategyChangeArgs(int stockId, int strategyId)
    {
        this.stockId = stockId;
        this.strategyId = strategyId;
    }
}
public class RefillCardsArgs
{
    public List<int> cardIds = new();
    public RefillCardsArgs(List<int> cardIds) => this.cardIds = cardIds;
}
public class DrawActionCardsArgs
{
    public List<int> drawed = new();
    public DrawActionCardsArgs(List<int> drawed) => this.drawed = drawed;
}
public class DrawTradeCardsArgs
{
    public List<int> drawed = new();
    public DrawTradeCardsArgs(List<int> drawed) => this.drawed = drawed;
}
public class DiscardActionCardsArgs
{
    public List<int> discards = new();
    public DiscardActionCardsArgs(List<int> discards) => this.discards = discards;
}
public class DiscardTradeCardsArgs
{
    public List<int> discards = new();
    public DiscardTradeCardsArgs(List<int> discards) => this.discards = discards;

}
public class CardEffectStartArgs
{
    public int cardId;
    public CardEffectStartArgs(int cardId) => this.cardId = cardId;
}
public class SetPointStateArgs
{
    public int stockId;
    public PointState state;
    public SetPointStateArgs(int stockId, PointState state)
    {
        this.stockId = stockId;
        this.state = state;
    }
}
public class ShowSidenoteArgs
{
    public List<string> messages;
    public SidenoteLayoutInfo layoutInfo;
}
public class PopupTipsArgs
{
    public string message = "";
    public TipsType tipsType = TipsType.Info;
    public float duration = -1;
    public PopupTipsArgs(string message, TipsType tipsType = TipsType.Info, float duration = -1)
    {
        this.message = message;
        this.tipsType = tipsType;
        this.duration = duration;
    }
}