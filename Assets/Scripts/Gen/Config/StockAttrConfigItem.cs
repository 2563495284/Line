using System.Collections.Generic;

namespace GameConfig
{
    public class StockAttrConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public StockAttrType UniqueKey { private set; get; }
        /// <summary>
        /// 词条id
        /// </summary>
        public StockAttrType Id { private set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 符号
        /// </summary>
        public string Key { private set; get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { private set; get; }
        /// <summary>
        /// 回合开始时效果
        /// </summary>
        public IReadOnlyList<string> Effect1 { private set; get; }
        /// <summary>
        /// 股价计算前效果
        /// </summary>
        public IReadOnlyList<string> Effect2 { private set; get; }
        /// <summary>
        /// 回合清算时效果
        /// </summary>
        public IReadOnlyList<string> Effect3 { private set; get; }

        public StockAttrConfigItem(StockAttrType uniqueKey, StockAttrType id, string name, string key, string desc, IReadOnlyList<string> effect1, IReadOnlyList<string> effect2, IReadOnlyList<string> effect3)
        {
            UniqueKey = uniqueKey;
            Id = id;
            Name = name;
            Key = key;
            Desc = desc;
            Effect1 = effect1;
            Effect2 = effect2;
            Effect3 = effect3;
        }
    }
}