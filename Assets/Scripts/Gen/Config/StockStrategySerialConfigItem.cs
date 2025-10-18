using System.Collections.Generic;

namespace GameConfig
{
    public class StockStrategySerialConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public int UniqueKey { private set; get; }
        /// <summary>
        /// 决策链系列
        /// </summary>
        public int SerialId { private set; get; }
        /// <summary>
        /// 决策链名称
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 决策链描述
        /// </summary>
        public string Desc { private set; get; }
        /// <summary>
        /// 影响类型（0中立，1积极，2消极）
        /// </summary>
        public int Emotion { private set; get; }

        public StockStrategySerialConfigItem(int uniqueKey, int serialId, string name, string desc, int emotion)
        {
            UniqueKey = uniqueKey;
            SerialId = serialId;
            Name = name;
            Desc = desc;
            Emotion = emotion;
        }
    }
}