
using System.Collections.Generic;
using UnityEngine;
public class Global : SingletonCom<Global>
{
    [Header("属性配置")]
    public List<AttrConfigItem> attrCfg = new List<AttrConfigItem>();
    public AttrConfigItem GetAttrCfg(EAttrType type) => attrCfg.Find(e => e.attributeType == type);
    public List<StockConfigItem> stockCfg = new();
    public StockConfigItem GetStockCfg(EStockType type) => stockCfg.Find(e => e.stockType == type);
    public List<CardConfigItem> cardCfgs = new();
    public CardConfigItem GetCardCfg(int id) => cardCfgs.Find(e => e.id == id);
    public List<EnemyConfigItem> enemyCfgs = new();
    public EnemyConfigItem GetEnemyCfg(int id) => enemyCfgs.Find(e => e.id == id);
}