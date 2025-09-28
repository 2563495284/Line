using System.Collections.Generic;

namespace GameConfig
{
    public class AttrConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public AttrType UniqueKey { private set; get; }
        /// <summary>
        /// 词条id
        /// </summary>
        public AttrType Id { private set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 标识
        /// </summary>
        public string Key { private set; get; }
        /// <summary>
        /// 格式化描述
        /// </summary>
        public string Desc { private set; get; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { private set; get; }
        /// <summary>
        /// 初始值
        /// </summary>
        public float OriginValue { private set; get; }

        public AttrConfigItem(AttrType uniqueKey, AttrType id, string name, string key, string desc, string icon, float originValue)
        {
            UniqueKey = uniqueKey;
            Id = id;
            Name = name;
            Key = key;
            Desc = desc;
            Icon = icon;
            OriginValue = originValue;
        }
    }
}