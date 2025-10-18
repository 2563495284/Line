using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameConfig;

public class EventMessageGrp : IIndexableElement<int>
{
    readonly int stockId;
    DList<EventImpactType, EventMessage> msgGrp;
    public EventMessageGrp(int stockId, IEnumerable<EventMessage> list)
    {
        this.stockId = stockId;
        msgGrp = new(list);
    }
    public int GetKey()
    {
        return stockId;
    }
    public EventMessage this[EventImpactType type]
    {
        get => msgGrp[type];
    }
}
public class EventMessage : IIndexableElement<EventImpactType>
{
    public EventMessageConfigItem Cfg { get; private set; }
    public EventMessage(EventMessageConfigItem cfg)
    {
        Cfg = cfg;
    }

    public EventImpactType GetKey()
    {
        return Cfg.ImpactType;
    }
}
public class GlobalConfig : Singleton<GlobalConfig>
{
    private DList<int, EventMessageGrp> eventMsgCfg = new();
    public void Init()
    {
        eventMsgCfg = new(Config.StockConfig.list.Select(e => new EventMessageGrp(e.Id,
            (Enum.GetValues(typeof(EventImpactType)) as EventImpactType[]).Select(f =>
            new EventMessage(Config.EventMessageConfig.list.FirstOrDefault(g => g.StockId == e.Id && g.ImpactType == f))
        ))));
    }
    public EventMessageConfigItem GetEventMsg(int stockId, EventImpactType type)
    {
        return eventMsgCfg[stockId][type].Cfg;
    }
}