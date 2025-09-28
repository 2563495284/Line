
using System.Collections.Generic;
using UnityEngine;
using GameConfig;
public class Global : SingletonCom<Global>
{
    [Header("属性配置")]
    public List<EnemyConfigItem> enemyCfgs = new();
    public EnemyConfigItem GetEnemyCfg(int id) => enemyCfgs.Find(e => e.id == id);
    public GameObject canvasPrefab;
    public int maxPriceHistoryCnt = 40;
}