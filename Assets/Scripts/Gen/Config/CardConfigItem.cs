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
        /// 目标类型
        /// </summary>
        public ReleaseTarget ReleaseMode { private set; get; }
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
        public IReadOnlyList<string> ReleaseEffect { private set; get; }
        /// <summary>
        /// 有向效果数组
        /// </summary>
        public IReadOnlyList<string> DrawedEffect { private set; get; }

        public CardConfigItem(int uniqueKey, int id, string faceImg, string title, string desc, ReleaseTarget releaseMode, int manaCost, CardType cardType, IReadOnlyList<string> releaseEffect, IReadOnlyList<string> drawedEffect)
        {
            UniqueKey = uniqueKey;
            Id = id;
            FaceImg = faceImg;
            Title = title;
            Desc = desc;
            ReleaseMode = releaseMode;
            ManaCost = manaCost;
            CardType = cardType;
            ReleaseEffect = releaseEffect;
            DrawedEffect = drawedEffect;
        }
    }
}