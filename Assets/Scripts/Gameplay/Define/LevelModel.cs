using System.Collections.Generic;
using System.Linq;
public class ShopInfo
{
    public List<DiceGoodsInfo> dices = new();
    public List<CardGoodsInfo> cards = new();
}
public class LevelModel
{
    public int curRouteIdx = 1;
    public int money = 1000000000;
    public int consumeCash = 0;
    public int cash = 0;
    public int maxBonusOneDay = 0;
    public List<PlayerAttrData> attrInfo = new();
    public DList<int, CardModel> cardDatas = new();
    public List<int> handCards_action = new();
    public List<int> drawCards_action = new();
    public List<int> discardCards_action = new();
    public List<int> handCards_trade = new();
    public List<int> drawCards_trade = new();
    public float industryPower = 1;
    public DList<int, StockModel> stocks = new();
    public LevelModel(LevelConfig cfg)
    {
        curRouteIdx = 1;
        IEnumerable<CardModel> actionCards = cfg.initialActionCards.Select(e => new CardModel(e));
        IEnumerable<CardModel> tradeCards = cfg.initialTradeCards.Select(e => new CardModel(e));
        drawCards_action = actionCards.Select(e => e.cardId).ToList();
        drawCards_trade = tradeCards.Select(e => e.cardId).ToList();
        cardDatas.Join(actionCards).Join(tradeCards);
        stocks = new(cfg.stockCfgs.Select(e => new StockModel(e)));
    }
    public ShopInfo GetShopInfo()
    {
        return new();
    }

}