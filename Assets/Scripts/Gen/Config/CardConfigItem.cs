using System.Collections.Generic;

namespace GameConfig
{
    public class CardConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public int UniqueKey { private set; get; }
        /// <summary>
        /// 卡牌配置ID
        /// </summary>
        public int Id { private set; get; }
        /// <summary>
        /// 符号
        /// </summary>
        public string Key { private set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { private set; get; }
        /// <summary>
        /// 阶段类型
        /// </summary>
        public PhaseType PhaseType { private set; get; }
        /// <summary>
        /// 释放目标类型
        /// </summary>
        public ReleaseTarget ReleaseTarget { private set; get; }
        /// <summary>
        /// 卡牌标签
        /// </summary>
        public CardTag Tag { private set; get; }
        /// <summary>
        /// 释放效果
        /// </summary>
        public IReadOnlyList<string> ReleaseEffect { private set; get; }

        public CardConfigItem(int uniqueKey, int id, string key, string name, string desc, PhaseType phaseType, ReleaseTarget releaseTarget, CardTag tag, IReadOnlyList<string> releaseEffect)
        {
            UniqueKey = uniqueKey;
            Id = id;
            Key = key;
            Name = name;
            Desc = desc;
            PhaseType = phaseType;
            ReleaseTarget = releaseTarget;
            Tag = tag;
            ReleaseEffect = releaseEffect;
        }
    }
}