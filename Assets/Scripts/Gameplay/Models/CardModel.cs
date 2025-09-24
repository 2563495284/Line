using System;
using System.Collections.Generic;
using System.Linq;
public interface ICardHolder
{

}
public enum ECardSelectState
{
    Ready,
    LackMana,
    LackRes,
}
public enum ECardReleaseState
{
    Ready,
    LackTarget,
    EffectFail

}
[Serializable]
public class CardModel : IEffectEmitter
{
    public int cfgId;
    public int cardId;
    public ICardHolder owner;
    public CardConfigItem Cfg => Global.Ins.GetCardCfg(cfgId);
    public string Desc => CardUtils.ProcessCardDescription(Cfg.desc).processedDescription;
    public string RichTextDesc => CardUtils.ProcessCardDescription(Cfg.desc).richTextDescription;
    public List<EAttrType> attrs => CardUtils.ProcessCardDescription(Cfg.desc).referencedAttributes;
    public ECardSelectState GetCardStateInRound()
    {
        if (GM.LevelData.mana < Cfg.manaCost)
            return ECardSelectState.LackMana;
        else
            return Cfg.effects.All(e => e.effect.CanBeEffect(GM.LevelData)) ? ECardSelectState.Ready : ECardSelectState.LackRes;
    }
    public CardModel(int cfgId, ICardHolder owner)
    {
        cardId = LevelController.CardCNT++;
        this.cfgId = cfgId;
        this.owner = owner;
    }
}