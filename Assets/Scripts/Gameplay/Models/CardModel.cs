using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameConfig;
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
public class CardModel : IEffectEmitter, IIndexableElement<int>
{
    public int cfgId;
    public int cardId;
    public CardConfigItem Cfg => Config.CardConfig.Get(cfgId);
    public string Desc => CardUtils.ProcessCardDescription(Cfg.Desc).processedDescription;
    public string RichTextDesc => CardUtils.ProcessCardDescription(Cfg.Desc).richTextDescription;
    public List<AttrType> attrs => CardUtils.ProcessCardDescription(Cfg.Desc).referencedAttributes;
    private List<Effect> cardEffects = new();
    private List<EffectWithTarget> cardEffectsWithTarget = new();
    public ECardSelectState GetCardStateInRound()
    {
        if (GM.LevelData.mana < Cfg.ManaCost)
            return ECardSelectState.LackMana;
        else
            return cardEffects.All(e => e.CanBeEffect(GM.LevelData)) ? ECardSelectState.Ready : ECardSelectState.LackRes;
    }
    public CardModel(int cfgId)
    {
        cardId = LevelController.CardCNT++;
        this.cfgId = cfgId;
        cardEffects = EffectFactory.GetEffects<Effect>(Cfg.Effects);
        cardEffectsWithTarget = EffectFactory.GetEffects<EffectWithTarget>(Cfg.EffectsWithTarget);
    }
    public IEnumerator Execute(IEffectEmitter from)
    {
        for (int i = 0; i < cardEffects.Count; i++)
            yield return cardEffects[i].Run(from);
    }
    public IEnumerator Execute(IEffectEmitter from, IEffectReceiver target)
    {
        yield return Execute(from);
        for (int i = 0; i < cardEffectsWithTarget.Count; i++)
            yield return cardEffectsWithTarget[i].Run(from, target);
    }

    public int GetKey()
    {
        return cardId;
    }
}