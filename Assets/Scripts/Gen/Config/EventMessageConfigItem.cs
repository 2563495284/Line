using System.Collections.Generic;

namespace GameConfig
{
    public class EventMessageConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public int UniqueKey { private set; get; }
        /// <summary>
        /// 消息id
        /// </summary>
        public int Id { private set; get; }
        /// <summary>
        /// 相关股票id
        /// </summary>
        public int StockId { private set; get; }
        /// <summary>
        /// 影响类型
        /// </summary>
        public EventImpactType ImpactType { private set; get; }
        /// <summary>
        /// 消息字符串
        /// </summary>
        public string Message { private set; get; }

        public EventMessageConfigItem(int uniqueKey, int id, int stockId, EventImpactType impactType, string message)
        {
            UniqueKey = uniqueKey;
            Id = id;
            StockId = stockId;
            ImpactType = impactType;
            Message = message;
        }
    }
}