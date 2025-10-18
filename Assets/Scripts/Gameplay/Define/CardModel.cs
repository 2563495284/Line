using GameConfig;

public class CardModel : IEffectSource, IIndexableElement<int>
{
    private static int CNT = 0;
    public int cardId;
    public int CfgId { get; private set; }
    public CardConfigItem Cfg => Config.CardConfig.Get(CfgId);
    public override string ToString()
    {
        return $"cardId: {cardId}, cfgId: {CfgId}";
    }
    public int GetKey()
    {
        return cardId;
    }
    public CardModel(int cfgId)
    {
        cardId = CNT++;
        CfgId = cfgId;
    }
}