using System.Collections.Generic;

namespace GameConfig
{
    public class StockStrategyConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public int UniqueKey { private set; get; }
        /// <summary>
        /// 决策链id
        /// </summary>
        public int Id { private set; get; }
        /// <summary>
        /// 决策链系列
        /// </summary>
        public int SerialId { private set; get; }
        /// <summary>
        /// 回合数
        /// </summary>
        public int Round { private set; get; }
        /// <summary>
        /// 属性（attr#回合数*延迟)
        /// </summary>
        public IReadOnlyList<string> Attrs { private set; get; }

        public StockStrategyConfigItem(int uniqueKey, int id, int serialId, int round, IReadOnlyList<string> attrs)
        {
            UniqueKey = uniqueKey;
            Id = id;
            SerialId = serialId;
            Round = round;
            Attrs = attrs;
        }
    }
}