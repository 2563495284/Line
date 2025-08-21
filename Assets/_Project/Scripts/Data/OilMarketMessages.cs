using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石油市场消息数据
/// </summary>
public static class OilMarketMessages
{
    /// <summary>
    /// 石油 利好消息（做多卡组）
    /// </summary>
    public static readonly List<string> BullishMessages = new List<string>
    {
        "OPEC+意外宣布额外减产，供应紧张预期升温",
        "重要产油地区局势再度紧张，市场担忧供应中断",
        "全球经济复苏迹象明显，原油需求预期大幅提升",
        "主要消费国战略石油储备库存降至历史低位",
        "重要制造业国家PMI强劲反弹，工业用油需求激增",
        "主要产油国石油设施遭遇技术故障，日产量锐减",
        "新兴市场大幅增加原油进口配额，亚洲需求旺盛",
        "炼油厂检修季延长，成品油供应吃紧",
        "新兴市场经济体联合增加石油采购",
        "主要货币指数大幅走弱，大宗商品投资吸引力上升",

        "飓风威胁重要海域石油平台，产量面临中断风险",
        "主要产油国石油出口再次受阻，供应缺口扩大",
        "全球航空业复苏超预期，航油需求强劲回升",
        "重要地区天然气短缺，替代能源需求推高油价",
        "关键产油国石油港口再次关闭，出口量大幅下降",
        "国际能源署下调全球石油产能预期",
        "主要产油国基础设施老化，维护成本飙升",
        "地缘政治风险升级，避险资金流入原油市场",
        "全球石油库存连续下降，供需平衡趋紧",
        "OPEC部长级会议释放鹰派信号",

        "主要页岩油产区钻井数量意外下降",
        "重要海域石油平台遭遇恶劣天气，生产暂停",
        "国际核谈判陷入僵局，制裁前景不明",
        "全球石化产业投资不足，长期供应担忧加剧",
        "主要石油期货交割库存创新低",
        "运输瓶颈导致原油物流成本上升",
        "可再生能源投资放缓，传统能源地位稳固",
        "新兴经济体工业化进程加速，能源消耗上升",
        "国际制裁措施影响全球石油贸易流向",
        "技术分析显示原油价格突破关键阻力位"
    };

    /// <summary>
    /// 石油 利空消息（做空卡组）
    /// </summary>
    public static readonly List<string> BearishMessages = new List<string>
    {
        "主要页岩油产区产量创历史新高，供应过剩担忧加剧",
        "全球经济衰退风险上升，石油需求前景堪忧",
        "OPEC内部分歧严重，减产协议执行不力",
        "主要经济体增长放缓，世界最大石油进口国需求疲软",
        "重要消费国释放战略石油储备，市场供应量激增",
        "全球疫情变种影响国际出行，航油需求大幅下降",
        "可再生能源技术突破，石油替代进程加速",
        "主要产油国石油出口增加，国际市场竞争加剧",
        "主要央行激进加息，强势货币打压大宗商品价格",
        "全球炼油产能扩张，原油加工需求增长有限",

        "电动汽车销量爆发式增长，汽油需求长期下滑",
        "重要经济体陷入技术性衰退，工业用油锐减",
        "国际贸易摩擦升级，全球供应链效率下降",
        "主要消费国战略储备达到满负荷",
        "石油输出国组织增产预期强烈",
        "主要消费国原油库存连续大幅增长",
        "全球航运业低迷，燃料油消费持续下滑",
        "天然气价格暴跌，替代能源竞争力增强",
        "国际金融市场动荡，投资者抛售大宗商品",
        "主要石油公司下调资本支出预期",

        "新兴市场货币危机影响石油购买力",
        "全球制造业PMI连续收缩，工业需求萎缩",
        "石油期货持仓量大幅下降，市场信心不足",
        "环保政策趋严，化石燃料使用受限",
        "技术进步降低石油开采成本，供应弹性增强",
        "地缘政治紧张局势缓解，风险溢价消散",
        "全球石油贸易路线优化，运输成本下降",
        "主要经济体通胀压力缓解，紧缩政策预期升温",
        "石油替代产品技术日趋成熟，市场占有率提升",
        "国际油价技术面显示头肩顶形态，下跌信号明确"
    };

    /// <summary>
    /// 石油 中性消息（中性卡组/无卡牌时）
    /// </summary>
    public static readonly List<string> NeutralMessages = new List<string>
    {
        "国际油价在技术区间内震荡，市场观望情绪浓厚",
        "石油市场多空因素交织，价格走势有待进一步明确",
        "分析师对油价后市走向存在分歧，建议谨慎操作",
        "原油期货成交量温和放大，市场活跃度有所提升",
        "石油库存数据符合预期，对市场影响相对有限",
        "国际能源署发布月度报告，维持供需平衡预测",
        "主要石油公司财报季临近，市场关注盈利表现",
        "地缘政治局势相对稳定，油价波动性有所收敛",
        "美元指数窄幅整理，对大宗商品影响暂时中性",
        "全球石油贸易流向正常，供应链运行平稳",

        "炼油利润率维持合理水平，上下游平衡发展",
        "石油期货曲线结构稳定，远期溢价温和收窄",
        "主要消费国库存处于季节性正常范围",
        "OPEC月报数据平稳，产量调整符合市场预期",
        "技术指标显示油价处于中性区域，方向尚不明确",
        "国际石油组织例行会议召开，维持现有政策不变",
        "全球经济数据喜忧参半，对能源需求影响中性",
        "石油运输费率稳定，物流成本变化不大",
        "可再生能源发展稳步推进，对传统能源冲击温和",
        "金融市场风险偏好中性，大宗商品资金流向平衡",

        "石油勘探投资保持稳定，长期供应预期未变",
        "主要经济体货币政策立场相对中性",
        "国际油价波动率回归历史均值水平",
        "石油化工产业链利润分配趋于合理",
        "全球石油消费增长与产能释放基本匹配",
        "环境政策执行进度符合既定时间表",
        "石油期货持仓结构保持相对均衡",
        "国际贸易环境总体稳定，石油流通正常",
        "主要产油区生产运营状况良好，无重大变故",
        "市场参与者情绪指数处于中性水平，等待新的催化因素"
    };

    // ================= 新增：钢铁与棉花的消息 =================

    public static readonly List<string> SteelBullishMessages = new List<string>
    {
        "钢铁行业订单大增，基建需求推动价格走强",
        "主要钢铁产区限产措施升级，供应收缩预期增强",
        "海外需求复苏，钢材出口报价上调",
        "矿石价格上涨传导至钢价，产业链利润修复",
        "制造业PMI回到扩张区间，钢材消费预期改善"
    };

    public static readonly List<string> SteelBearishMessages = new List<string>
    {
        "粗钢产量持续走高，库存累积压制价格",
        "房地产投资放缓，钢材终端需求转弱",
        "海外竞争加剧，钢材出口订单下滑",
        "原料成本回落，成材价格回吐涨幅",
        "环保限产边际放松，供给压力抬头"
    };

    public static readonly List<string> SteelNeutralMessages = new List<string>
    {
        "钢材现货与期货价差收敛，市场情绪中性",
        "钢厂开工率稳定，供需基本平衡",
        "社会库存去化放缓，价格区间震荡",
        "终端采购按需进行，短期指引有限",
        "行业利润维持常态，市场观望情绪浓厚"
    };

    public static readonly List<string> CottonBullishMessages = new List<string>
    {
        "新季棉花减产预期增强，供给紧张推升价格",
        "纺织订单回暖，下游补库意愿提升",
        "主要产区天气不佳，棉花采收受阻",
        "国际棉价走强带动内盘联动上涨",
        "库存持续去化，现货报价上调"
    };

    public static readonly List<string> CottonBearishMessages = new List<string>
    {
        "纺织行业开机率回落，原料需求走弱",
        "新花集中上市，阶段性供应压力显现",
        "国际订单不足，出口不及预期",
        "替代纤维性价比提升，挤压棉花需求",
        "库存累积加快，贸易商报价松动"
    };

    public static readonly List<string> CottonNeutralMessages = new List<string>
    {
        "棉花期现价差稳定，市场博弈加剧",
        "上下游维持刚需采购，价格窄幅波动",
        "产销两端节奏平稳，短期指引不足",
        "宏观扰动有限，品种跟随盘面震荡",
        "贸易流向正常，供需边际变化不大"
    };

    /// <summary>
    /// 根据股票类型与事件类型获取随机消息
    /// </summary>
    public static string GetRandomMessage(EStockType stockType, EEventCardType eventType)
    {
        List<string> messages;

        switch (stockType)
        {
            case EStockType.Steel:
                messages = eventType switch
                {
                    EEventCardType.Bull => SteelBullishMessages,
                    EEventCardType.Bear => SteelBearishMessages,
                    _ => SteelNeutralMessages
                };
                break;
            case EStockType.Cotton:
                messages = eventType switch
                {
                    EEventCardType.Bull => CottonBullishMessages,
                    EEventCardType.Bear => CottonBearishMessages,
                    _ => CottonNeutralMessages
                };
                break;
            case EStockType.Oil:
            default:
                messages = eventType switch
                {
                    EEventCardType.Bull => BullishMessages,
                    EEventCardType.Bear => BearishMessages,
                    _ => NeutralMessages
                };
                break;
        }

        if (messages.Count == 0) return "市场信息暂无更新";
        return messages[Random.Range(0, messages.Count)];
    }

    /// <summary>
    /// 兼容旧接口：默认以石油为品类
    /// </summary>
    public static string GetRandomMessage(EEventCardType eventType)
    {
        return GetRandomMessage(EStockType.Oil, eventType);
    }

    /// <summary>
    /// 获取指定类型的所有消息（兼容旧接口，仅石油）
    /// </summary>
    public static List<string> GetAllMessages(EEventCardType eventType)
    {
        return eventType switch
        {
            EEventCardType.Bull => new List<string>(BullishMessages),
            EEventCardType.Bear => new List<string>(BearishMessages),
            EEventCardType.Neutral => new List<string>(NeutralMessages),
            _ => new List<string>(NeutralMessages)
        };
    }
}