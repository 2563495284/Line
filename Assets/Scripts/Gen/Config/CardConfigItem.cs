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
        /// 卡面名
        /// </summary>
        public string FaceImg { private set; get; }
        /// <summary>
        /// 卡牌标题
        /// </summary>
        public string Title { private set; get; }
        /// <summary>
        /// 卡牌描述
        /// </summary>
        public string Desc { private set; get; }
        /// <summary>
        /// 释放类型（0无目标，1对股票释放）
        /// </summary>
        public ReleaseMode ReleaseMode { private set; get; }
        /// <summary>
        /// 能量消耗
        /// </summary>
        public int ManaCost { private set; get; }
        /// <summary>
        /// 卡牌类型
        /// </summary>
        public CardType CardType { private set; get; }
        /// <summary>
        /// 效果数组
        /// </summary>
        public IReadOnlyList<string> Effects { private set; get; }
        /// <summary>
        /// 有向效果数组
        /// </summary>
        public IReadOnlyList<string> EffectsWithTarget { private set; get; }

        public CardConfigItem(int uniqueKey, int id, string faceImg, string title, string desc, ReleaseMode releaseMode, int manaCost, CardType cardType, IReadOnlyList<string> effects, IReadOnlyList<string> effectsWithTarget)
        {
            UniqueKey = uniqueKey;
            Id = id;
            FaceImg = faceImg;
            Title = title;
            Desc = desc;
            ReleaseMode = releaseMode;
            ManaCost = manaCost;
            CardType = cardType;
            Effects = effects;
            EffectsWithTarget = effectsWithTarget;
        }
    }
}