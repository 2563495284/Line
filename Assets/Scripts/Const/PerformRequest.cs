using System.Collections.Generic;

public static class PerformRequest
{
    public static readonly string
        Play_PresentCard = "Play_PresentCard",//PlayCardArgs
        Play_EffectCard = "Play_EffectCard",//PlayCardArgs
        Play_DiscardCard = "Play_DiscardCard",//PlayCardArgs
        DrawCards = "DrawCards",//OperateCardsArgs
        DiscardCardAll = "DiscardCards",//OperateCardsArgs
        RefillCards = "RefillCards";
}
public class PlayCardArgs
{
    public CardModel card;
    public IEffectReceiver receiver;
    public PlayCardArgs(CardModel card, IEffectReceiver receiver)
    {
        this.card = card;
        this.receiver = receiver;
    }
}
public class DeckOPArgs
{
    public List<CardModel> cards;
    public DeckOPArgs(List<CardModel> cards)
    {
        this.cards = cards;
    }
}