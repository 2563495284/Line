
using System.Collections.Generic;
public class EnemyModel : IEffectEmitter

{
    public int cfgId = 0;
    public int enemyId = 0;
    public EnemyConfigItem Cfg => Global.Ins.GetEnemyCfg(cfgId);
    public EStrategyType curStrategy;
    public List<CardModel> drawCards = new();
    public List<CardModel> handCards = new();
    public List<CardModel> discardCards = new();
    public EnemyModel(int cfgId)
    {
        enemyId = LevelController.EnemyCNT++;
        this.cfgId = cfgId;
        curStrategy = Cfg.strategyType;
    }
}