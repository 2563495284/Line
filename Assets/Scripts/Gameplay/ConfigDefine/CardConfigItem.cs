using SerializeReferenceEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
public enum CardType
{
    Trade,  //交易
    Futures, //期货
    Operate,//操盘
    Empowerment//赋能
}
[Serializable]
public class CardEffect
{
    [SerializeField, SR] public Effect effect;
}
public class CardEffectWithTarget
{
    [SerializeField, SR]
    public EffectWithTarget effect;
}
public enum ECardReleaseType
{
    TargetToStock,
    NoTarget,
}
[CreateAssetMenu(menuName = "ConfigData/Card")]
public class CardConfigItem : ScriptableObject
{
    public int id;
    public Sprite image;
    public string title;
    public string desc;
    [field: SerializeReference, SR] public Effect2 manualTargetEffect = null;
    public List<CardEffectWithTarget> effectsWithTarget = new();
    public List<CardEffect> effects = new();
    public ECardReleaseType releaseType;
    public int manaCost;
    public CardType type;

}