using System.Collections.Generic;
using System.Linq;
using GameConfig;

public class StockAttrData : IEffectSource, IIndexableElement<int>
{
    private static int CNT = 0;
    public int Id { get; private set; }
    private StockAttrType cfgId;
    public bool isExtra = false;
    public int round = -1;
    public int delay = 0;
    public StockAttrConfigItem Cfg => Config.StockAttrConfig.Get(cfgId);
    public int StockId { get; private set; }
    public List<EffectBase> Effects { get; private set; }
    public StockAttrData(int stockId, StockAttrType attrType, int round = -1, int delay = 0, bool isExtra = false)
    {
        Id = CNT++;
        this.StockId = stockId;
        this.cfgId = attrType;
        this.isExtra = isExtra;
        this.round = round;
        this.delay = delay;
    }
    public bool HasEffectReady(EStockAttrApplyTiming timing)
    {
        return Effects.Any(e => timing switch
            {
                EStockAttrApplyTiming.AfterSettlement => e is IAutoEffect_RoundEnd,
                EStockAttrApplyTiming.RoundStart => e is IAutoEffect_RoundStart,
                EStockAttrApplyTiming.BeforeSettlement => e is IAutoEffect_BeforeCalc,
                _ => false
            });
    }
    public void ExitRound()
    {
        if (delay > 0)
            delay--;
        else if (round > 0)
            round--;

    }
    public int GetKey()
    {
        return Id;
    }
}