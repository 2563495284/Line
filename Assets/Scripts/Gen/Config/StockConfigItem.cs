using System.Collections.Generic;

namespace GameConfig
{
    public class StockConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public int UniqueKey { private set; get; }
        /// <summary>
        /// 股票id
        /// </summary>
        public int Id { private set; get; }
        /// <summary>
        /// 股票名
        /// </summary>
        public string StockName { private set; get; }
        /// <summary>
        /// 符号
        /// </summary>
        public string Symbol { private set; get; }
        /// <summary>
        /// 基础波动性
        /// </summary>
        public float BaseVolatility { private set; get; }
        /// <summary>
        /// 初始价格
        /// </summary>
        public float OriginPrice { private set; get; }

        public StockConfigItem(int uniqueKey, int id, string stockName, string symbol, float baseVolatility, float originPrice)
        {
            UniqueKey = uniqueKey;
            Id = id;
            StockName = stockName;
            Symbol = symbol;
            BaseVolatility = baseVolatility;
            OriginPrice = originPrice;
        }
    }
}